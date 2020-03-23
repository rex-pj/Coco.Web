﻿using Coco.Business.Contracts;
using Coco.Business.Mapping;
using Coco.Business.ValidationStrategies;
using Coco.Contract;
using Coco.Entities.Domain.Identity;
using Coco.Entities.Dtos.User;
using Coco.Entities.Dtos.General;
using Coco.IdentityDAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Coco.Entities.Enums;

namespace Coco.Business.Implementation.UserBusiness
{
    public partial class UserBusiness : IUserBusiness
    {
        #region Fields/Properties
        private readonly IDbContext _identityContext;
        private readonly IRepository<UserInfo> _userInfoRepository;
        private readonly IRepository<User> _userRepository;
        private readonly ValidationStrategyContext _validationStrategyContext;
        private readonly IMapper _mapper;
        #endregion

        #region Ctor
        public UserBusiness(IdentityDbContext identityContext, IRepository<User> userRepository,
            ValidationStrategyContext validationStrategyContext,
            IMapper mapper,
            IRepository<UserInfo> userInfoRepository)
        {
            _identityContext = identityContext;
            _mapper = mapper;
            _userRepository = userRepository;
            _userInfoRepository = userInfoRepository;
            _validationStrategyContext = validationStrategyContext;
        }
        #endregion

        #region CRUD
        public void Delete(long id)
        {
            var user = _userRepository.Find(id);
            user.IsActived = false;

            _userRepository.Update(user);
            _identityContext.SaveChanges();
        }

        public async Task<bool> ActiveAsync(long id)
        {
            var user = _userRepository.Find(id);
            if (user.IsActived)
            {
                throw new InvalidOperationException($"User with email: {user.Email} is already actived");
            }

            user.IsActived = true;
            user.IsEmailConfirmed = true;
            user.StatusId = (byte)UserStatusEnum.Actived;
            _userRepository.Update(user);
            await _identityContext.SaveChangesAsync();

            return true;
        }

        public async Task<UpdatePerItemDto> UpdateInfoItemAsync(UpdatePerItemDto model)
        {
            if (model.PropertyName == null)
            {
                throw new ArgumentNullException(nameof(model.PropertyName));
            }

            if (model.Key == null)
            {
                throw new ArgumentNullException(nameof(model.Key));
            }

            var userInfo = _userInfoRepository.Find(model.Key);

            if (userInfo == null)
            {
                throw new ArgumentNullException(nameof(userInfo));
            }

            _validationStrategyContext.SetStrategy(new UserInfoItemUpdationValidationStratergy(_validationStrategyContext));
            bool canUpdate = _validationStrategyContext.Validate(model);

            if (!canUpdate)
            {
                throw new ArgumentException(model.PropertyName);
            }

            if (userInfo.User != null)
            {
                userInfo.User.UpdatedDate = DateTime.UtcNow;
                userInfo.User.UpdatedById = userInfo.Id;
            }
            _userInfoRepository.UpdateByName(userInfo, model.Value, model.PropertyName, true);
            await _identityContext.SaveChangesAsync();

            return model;
        }

        public async Task<UserIdentifierUpdateDto> UpdateIdentifierAsync(UserIdentifierUpdateDto model)
        {
            _validationStrategyContext.SetStrategy(new UserProfileUpdateValidationStratergy());
            bool canUpdate = _validationStrategyContext.Validate(model);
            if (!canUpdate)
            {
                foreach(var item in _validationStrategyContext.Errors)
                {
                    throw new ArgumentNullException(item.Message);
                }
            }

            var user = await _userRepository.FindAsync(model.Id);

            user.UpdatedById = model.Id;
            user.UpdatedDate = DateTime.UtcNow;
            user.Lastname = model.Lastname;
            user.Firstname = model.Firstname;
            user.DisplayName = model.DisplayName;

            _userRepository.Update(user);
            await _identityContext.SaveChangesAsync();

            return model;
        }
        #endregion

        #region GET
        public async Task<UserDto> FindUserByEmail(string email)
        {
            email = email.ToLower();

            var user = await _userRepository
                .GetAsNoTracking(x => x.Email.Equals(email))
                .Select(UserMapping.UserModelSelector)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<UserDto> FindUserByUsername(string username)
        {
            username = username.ToLower();

            var user = await _userRepository
                .GetAsNoTracking(x => x.Email.Equals(username))
                .Select(UserMapping.UserModelSelector)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<UserDto> FindByIdAsync(long id)
        {
            var existUser = await _userRepository
                .GetAsNoTracking(x => x.Id.Equals(id))
                .Select(UserMapping.UserModelSelector)
                .FirstOrDefaultAsync();

            return existUser;
        }

        public async Task<UserFullDto> FindFullByIdAsync(long id)
        {
            var existUser = await _userRepository
                .GetAsNoTracking(x => x.Id.Equals(id))
                .Select(UserMapping.FullUserModelSelector)
                .FirstOrDefaultAsync();

            return existUser;
        }
        #endregion
    }
}