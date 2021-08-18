﻿using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Camino.IdentityManager.Contracts.Core;
using System.Security.Claims;
using Camino.Core.Contracts.IdentityManager;
using Camino.Core.Domain.Identities;

namespace Camino.IdentityManager
{
    public class ApplicationUserManager<TUser> : UserManager<TUser>, IUserManager<TUser> where TUser : ApplicationUser
    {
        public ApplicationUserManager(IUserStore<TUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<TUser> passwordHasher,
            IEnumerable<IUserValidator<TUser>> userValidators,
            IEnumerable<IPasswordValidator<TUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<TUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services,
                 logger)
        {
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
            return await userEncryptionStore.EncryptUserId(userId);
        }

        public virtual async Task<long> DecryptUserIdAsync(string userIdentityId)
        {
            var userEncryptionStore = GetUserEncryptionStore();
            return await userEncryptionStore.DecryptUserId(userIdentityId);
        }

        public virtual string NewSecurityStamp()
        {
            byte[] bytes = new byte[20];
            RandomNumberGenerator.Fill(bytes);
            return Base32.ToBase32(bytes);
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
            return await userPolicyStore.HasPolicyAsync(user, NormalizeName(policy), CancellationToken);
        }

        public async Task<TUser> FindByIdentityIdAsync(string userIdentityId)
        {
            var cast = Store as IUserStore<TUser>;
            if (cast == null)
            {
                throw new NotSupportedException("Store is not UserEncryptionStore");
            }

            var userId = await DecryptUserIdAsync(userIdentityId);
            return await cast.FindByIdAsync(userId.ToString(), CancellationToken);
        }
    }
}
