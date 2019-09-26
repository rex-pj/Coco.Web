﻿using Coco.Api.Framework.Models;
using Coco.Entities.Enums;
using Coco.Entities.Model.User;
using Coco.Entities.Model.General;
using System;
using System.Threading.Tasks;
using Coco.Api.Framework.SessionManager.Core;

namespace Coco.Api.Framework.SessionManager.Contracts
{
    public interface IUserManager<TUser> : IDisposable where TUser : class
    {
        IdentityOptions Options { get; set; }
        Task<ApiResult> CreateAsync(TUser user);
        Task<string> GetUserNameAsync(TUser user);
        Task<string> GetEmailAsync(TUser user);
        Task<TUser> FindByEmailAsync(string email, bool includeInActived = false);
        Task<TUser> FindByNameAsync(string userName);
        Task<string> GetUserIdAsync(TUser user);
        Task<ApiResult> CheckPasswordAsync(TUser user, string password);
        TUser GetLoggingUser(string userIdentityId, string authenticationToken);
        Task<UserFullModel> GetFullByHashIdAsync(string userIdentityId);
        Task<ApiResult> UpdateInfoItemAsync(UpdatePerItemModel model, string userIdentityId, string token);
        Task<ApiResult> UpdateAvatarAsync(UpdateUserPhotoModel model, long userId);
        Task<ApiResult> UpdateCoverAsync(UpdateUserPhotoModel model, long userId);
        Task<ApiResult> DeleteUserPhotoAsync(long userId, UserPhotoTypeEnum userPhotoType);
        Task<ApiResult> UpdateUserProfileAsync(ApplicationUser user, string userIdentityId, string token);
        Task<ApiResult> ChangePasswordAsync(long userId, string currentPassword, string newPassword);
        Task<ApiResult> ForgotPasswordAsync(string email);
        Task<bool> IsEmailConfirmedAsync(ApplicationUser user);
    }
}