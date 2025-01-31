﻿using Camino.Infrastructure.AspNetCore.Controllers;
using Camino.Infrastructure.AspNetCore.Models;
using Module.Authentication.WebAdmin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Camino.Shared.Enums;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Camino.Application.Contracts.AppServices.Users;
using Camino.Shared.Configuration.Options;
using Camino.Infrastructure.Http.Interfaces;
using Camino.Shared.Constants;
using Camino.Application.Contracts.AppServices.Users.Dtos;
using Camino.Application.Contracts;
using Camino.Infrastructure.Identity.Attributes;

namespace Module.Authentication.WebAdmin.Controllers
{
    public class UserController : BaseAuthController
    {
        private readonly IUserAppService _userAppService;
        private readonly IHttpHelper _httpHelper;
        private readonly PagerOptions _pagerOptions;
        private const int _defaultPageSelection = 1;

        public UserController(IUserAppService userAppService, IHttpContextAccessor httpContextAccessor,
            IHttpHelper httpHelper, IOptions<PagerOptions> pagerOptions)
            : base(httpContextAccessor)
        {
            _httpHelper = httpHelper;
            _userAppService = userAppService;
            _pagerOptions = pagerOptions.Value;
        }

        [ApplicationAuthorize(AuthorizePolicies.CanReadUser)]
        [PopulatePermissions("User", PolicyMethods.CanUpdate, PolicyMethods.CanDelete)]
        public async Task<IActionResult> Index(UserFilterModel filter)
        {
            var userPageList = await _userAppService.GetAsync(new UserFilter
            {
                Address = filter.Address,
                BirthDateFrom = filter.BirthDateFrom,
                BirthDateTo = filter.BirthDateTo,
                CountryId = filter.CountryId,
                CreatedById = filter.CreatedById,
                CreatedDateFrom = filter.CreatedDateFrom,
                CreatedDateTo = filter.CreatedDateTo,
                GenderId = filter.GenderId,
                IsEmailConfirmed = filter.IsEmailConfirmed,
                Page = filter.Page,
                PageSize = _pagerOptions.PageSize,
                PhoneNumber = filter.PhoneNumber,
                Keyword = filter.Search,
                StatusId = filter.StatusId,
                UpdatedById = filter.UpdatedById,
                CanGetDeleted = true,
                CanGetInactived = true
            });
            var users = userPageList.Collections.Select(x => new UserModel
            {
                Address = x.Address,
                UpdatedById = x.UpdatedById,
                StatusId = (UserStatuses)x.StatusId,
                BirthDate = x.BirthDate,
                CreatedById = x.CreatedById,
                CreatedDate = x.CreatedDate,
                CountryCode = x.CountryCode,
                CountryId = x.CountryId,
                CountryName = x.CountryName,
                Description = x.Description,
                DisplayName = x.DisplayName,
                Email = x.Email,
                Firstname = x.Firstname,
                Lastname = x.Lastname,
                GenderId = x.GenderId,
                GenderLabel = x.GenderLabel,
                Id = x.Id,
                IsEmailConfirmed = x.IsEmailConfirmed,
                PhoneNumber = x.PhoneNumber,
                UpdatedDate = x.UpdatedDate
            });
            var userPage = new PageListModel<UserModel>(users)
            {
                Filter = filter,
                TotalPage = userPageList.TotalPage,
                TotalResult = userPageList.TotalResult
            };

            if (_httpHelper.IsAjaxRequest(Request))
            {
                return PartialView("_UserTable", userPage);
            }

            return View(userPage);
        }

        [ApplicationAuthorize(AuthorizePolicies.CanReadUser)]
        public async Task<IActionResult> Detail(long id)
        {
            var userResult = await _userAppService.FindFullByIdAsync(new IdRequestFilter<long>
            {
                Id = id,
                CanGetDeleted = true,
                CanGetInactived = true
            });
            var user = new UserModel
            {
                Address = userResult.Address,
                UpdatedById = userResult.UpdatedById,
                StatusId = (UserStatuses)userResult.StatusId,
                StatusLabel = userResult.StatusLabel,
                BirthDate = userResult.BirthDate,
                CreatedById = userResult.CreatedById,
                CreatedDate = userResult.CreatedDate,
                CountryCode = userResult.CountryCode,
                CountryId = userResult.CountryId,
                CountryName = userResult.CountryName,
                Description = userResult.Description,
                DisplayName = userResult.DisplayName,
                Email = userResult.Email,
                Firstname = userResult.Firstname,
                Lastname = userResult.Lastname,
                GenderId = userResult.GenderId,
                GenderLabel = userResult.GenderLabel,
                Id = userResult.Id,
                IsEmailConfirmed = userResult.IsEmailConfirmed,
                PhoneNumber = userResult.PhoneNumber,
                UpdatedDate = userResult.UpdatedDate
            };

            return View(user);
        }

        [HttpGet]
        [ApplicationAuthorize(AuthorizePolicies.CanReadUser)]
        public async Task<IActionResult> Search(string q, List<long> currentUserIds)
        {
            var users = await _userAppService.SearchAsync(new UserFilter
            {
                Keyword = q,
                PageSize = _pagerOptions.PageSize,
                Page = _defaultPageSelection
            }, currentUserIds);
            if (users == null || !users.Any())
            {
                return Json(new
                {
                    Items = new List<Select2ItemModel>()
                });
            }

            var userModels = users.Select(x => new Select2ItemModel
            {
                Id = x.Id.ToString(),
                Text = x.Lastname + " " + x.Firstname
            });

            return Json(userModels);
        }

        [HttpPost]
        [ApplicationAuthorize(AuthorizePolicies.CanUpdateUser)]
        public async Task<IActionResult> TemporaryDelete(UserIdRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToErrorPage();
            }

            var isDeleted = await _userAppService.SoftDeleteAsync(request.Id, LoggedUserId);

            if (!isDeleted)
            {
                return RedirectToErrorPage();
            }

            if (request.ShouldKeepDetailPage)
            {
                return RedirectToAction(nameof(Detail), new { id = request.Id });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ApplicationAuthorize(AuthorizePolicies.CanUpdateUser)]
        public async Task<IActionResult> Deactivate(UserIdRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToErrorPage();
            }

            var isInactived = await _userAppService.DeactivateAsync(request.Id, LoggedUserId);

            if (!isInactived)
            {
                return RedirectToErrorPage();
            }

            if (request.ShouldKeepDetailPage)
            {
                return RedirectToAction(nameof(Detail), new { id = request.Id });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ApplicationAuthorize(AuthorizePolicies.CanUpdateUser)]
        public async Task<IActionResult> Active(UserIdRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToErrorPage();
            }

            var isActived = await _userAppService.ActiveAsync(request.Id, LoggedUserId);

            if (!isActived)
            {
                return RedirectToErrorPage();
            }

            if (request.ShouldKeepDetailPage)
            {
                return RedirectToAction(nameof(Detail), new { id = request.Id });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ApplicationAuthorize(AuthorizePolicies.CanUpdateUser)]
        public async Task<IActionResult> Confirm(UserIdRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToErrorPage();
            }

            var isConfirmed = await _userAppService.ConfirmAsync(request.Id, LoggedUserId);

            if (!isConfirmed)
            {
                return RedirectToErrorPage();
            }

            if (request.ShouldKeepDetailPage)
            {
                return RedirectToAction(nameof(Detail), new { id = request.Id });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}