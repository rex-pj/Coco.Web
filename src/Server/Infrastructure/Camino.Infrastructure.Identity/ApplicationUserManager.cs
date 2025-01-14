﻿using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Security.Claims;
using Camino.Infrastructure.Identity.Interfaces;
using Camino.Infrastructure.Identity.Core;
using Camino.Shared.Constants;
using Camino.Shared.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Camino.Infrastructure.Identity
{
    public class ApplicationUserManager<TUser> : UserManager<TUser>, IUserManager<TUser> where TUser : ApplicationUser
    {
        private readonly IJwtHelper _jwtHelper;
        public ApplicationUserManager(IUserStore<TUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<TUser> passwordHasher,
            IEnumerable<IUserValidator<TUser>> userValidators,
            IEnumerable<IPasswordValidator<TUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<TUser>> logger,
            IJwtHelper jwtHelper)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services,
                 logger)
        {
            _jwtHelper = jwtHelper;
        }

        private IUserEncryptionStore<TUser> GetUserEncryptionStore()
        {
            var cast = Store as IUserEncryptionStore<TUser>;
            if (cast == null)
            {
                throw new NotSupportedException("Store is not UserEncryptionStore");
            }
            return cast;
        }

        private IUserPolicyStore<TUser> GetUserPolicyStore()
        {
            var cast = Store as IUserPolicyStore<TUser>;
            if (cast == null)
            {
                throw new NotSupportedException("Store is not UserRoleStore");
            }
            return cast;
        }

        public virtual async Task<string> EncryptUserIdAsync(long userId)
        {
            var userEncryptionStore = GetUserEncryptionStore();
            return await userEncryptionStore.EncryptUserIdAsync(userId);
        }

        public virtual async Task<long> DecryptUserIdAsync(string userIdentityId)
        {
            var userEncryptionStore = GetUserEncryptionStore();
            return await userEncryptionStore.DecryptUserIdAsync(userIdentityId);
        }

        public virtual string EncryptUserId(long userId)
        {
            var userEncryptionStore = GetUserEncryptionStore();
            return userEncryptionStore.EncryptUserId(userId);
        }

        public long DecryptUserId(string userIdentityId)
        {
            var userEncryptionStore = GetUserEncryptionStore();
            return userEncryptionStore.DecryptUserId(userIdentityId);
        }

        public virtual string NewSecurityStamp()
        {
            byte[] bytes = new byte[20];
            RandomNumberGenerator.Fill(bytes);
            return Base32Utils.ToBase32(bytes);
        }

        public virtual async Task<bool> HasPolicyAsync(ClaimsPrincipal user, string policy)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var appUser = await GetUserAsync(user);
            return await HasPolicyAsync(appUser, policy);
        }

        public virtual async Task<bool> HasPolicyAsync(TUser user, string policy)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var userPolicyStore = GetUserPolicyStore();
            return await userPolicyStore.HasPolicyAsync(user, policy, CancellationToken);
        }

        public async Task<TUser> FindByIdentityIdAsync(string userIdentityId)
        {
            if (string.IsNullOrEmpty(userIdentityId))
            {
                throw new ArgumentNullException(nameof(userIdentityId));
            }

            var userId = await DecryptUserIdAsync(userIdentityId);
            return await FindByIdAsync(userId);
        }

        public async Task<TUser> FindByIdAsync(long userId)
        {
            if (Store == null)
            {
                throw new NotSupportedException("Store is not UserEncryptionStore");
            }

            return await Store.FindByIdAsync(userId.ToString(), CancellationToken);
        }

        public virtual Task<ApplicationUserToken> GetUserTokenByValueAsync(TUser user, string value, string tokenName)
        {
            ThrowIfDisposed();
            var store = GetUserTokenStore();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (tokenName == null)
            {
                throw new ArgumentNullException(nameof(tokenName));
            }

            return store.FindTokenByValueAsync(user, value, tokenName);
        }

        public virtual async Task RemoveAuthenticationTokenByValueAsync(long userId, string value)
        {
            ThrowIfDisposed();
            var store = GetUserTokenStore();
            if (userId <= 0)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            await store.RemoveAuthenticationTokenByValueAsync(userId, value);
        }

        public async Task<ClaimsIdentity> GetPrincipalFromExpiredTokenAsync(string token)
        {
            var jwtSecurityToken = _jwtHelper.GetExpiredToken(token);
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            var userIdentityId = jwtSecurityToken.Claims.First(x => x.Type == HttpHeaders.UserIdentityClaimKey).Value;
            var userId = await DecryptUserIdAsync(userIdentityId);
            var claimIdentity = new ClaimsIdentity(jwtSecurityToken.Claims, JwtBearerDefaults.AuthenticationScheme);
            claimIdentity.AddClaim(new Claim(HttpHeaders.UserIdClaimKey, userId.ToString()));

            return claimIdentity;
        }

        public async Task<ClaimsIdentity> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(token);
            }

            try
            {
                var jwtSecurityToken = _jwtHelper.GetSecurityToken(token);
                if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                {
                    var userIdentityId = jwtSecurityToken.Claims.First(x => x.Type == HttpHeaders.UserIdentityClaimKey).Value;
                    var userId = await DecryptUserIdAsync(userIdentityId);
                    var claimIdentity = new ClaimsIdentity(jwtSecurityToken.Claims, JwtBearerDefaults.AuthenticationScheme);
                    claimIdentity.AddClaim(new Claim(HttpHeaders.UserIdClaimKey, userId.ToString()));

                    return claimIdentity;
                }
            }
            catch (SecurityTokenExpiredException ex)
            {
                throw new CaminoAuthenticationException(ex.Message, ErrorMessages.TokenExpiredException);
            }

            return new ClaimsIdentity();
        }

        private IUserTokenStore<TUser> GetUserTokenStore()
        {
            var cast = Store as IUserTokenStore<TUser>;
            if (cast == null)
            {
                throw new NotSupportedException("StoreNotIUserTokenStore");
            }
            return cast;
        }
    }
}
