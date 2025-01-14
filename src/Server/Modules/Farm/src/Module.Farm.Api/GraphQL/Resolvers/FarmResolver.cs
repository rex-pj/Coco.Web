﻿using Camino.Infrastructure.GraphQL.Resolvers;
using Camino.Infrastructure.AspNetCore.Models;
using Module.Farm.Api.GraphQL.Resolvers.Contracts;
using Module.Farm.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Camino.Application.Contracts.AppServices.Farms;
using Camino.Infrastructure.Identity.Interfaces;
using Camino.Shared.Configuration.Options;
using Camino.Infrastructure.Identity.Core;
using Camino.Application.Contracts;
using Camino.Application.Contracts.AppServices.Farms.Dtos;

namespace Module.Farm.Api.GraphQL.Resolvers
{
    public class FarmResolver : BaseResolver, IFarmResolver
    {
        private readonly IFarmAppService _farmAppService;
        private readonly IUserManager<ApplicationUser> _userManager;
        private readonly PagerOptions _pagerOptions;
        private const int _defaultPageSelection = 1;

        public FarmResolver(IFarmAppService farmAppService, IUserManager<ApplicationUser> userManager,
            IOptions<PagerOptions> pagerOptions)
            : base()
        {
            _farmAppService = farmAppService;
            _userManager = userManager;
            _pagerOptions = pagerOptions.Value;
        }

        public async Task<IEnumerable<SelectOption>> SelectUserFarmsAsync(ClaimsPrincipal claimsPrincipal, FarmSelectFilterModel criterias)
        {
            if (criterias == null)
            {
                criterias = new FarmSelectFilterModel();
            }

            var currentUserId = GetCurrentUserId(claimsPrincipal);
            var filter = new SelectFilter()
            {
                CreatedById = currentUserId,
                CurrentIds = criterias.CurrentIds,
                Keyword = criterias.Query
            };

            var farms = await _farmAppService.SelectAsync(filter, _defaultPageSelection, _pagerOptions.PageSize);
            if (farms == null || !farms.Any())
            {
                return new List<SelectOption>();
            }

            var farmSeletions = farms
                .Select(x => new SelectOption
                {
                    Id = x.Id.ToString(),
                    Text = x.Name
                });

            return farmSeletions;
        }

        public async Task<FarmPageListModel> GetUserFarmsAsync(ClaimsPrincipal claimsPrincipal, FarmFilterModel criterias)
        {
            if (criterias == null)
            {
                criterias = new FarmFilterModel();
            }

            if (string.IsNullOrEmpty(criterias.UserIdentityId))
            {
                return new FarmPageListModel(new List<FarmModel>())
                {
                    Filter = criterias
                };
            }

            var currentUserId = GetCurrentUserId(claimsPrincipal);
            var userId = await _userManager.DecryptUserIdAsync(criterias.UserIdentityId);
            var filterRequest = new FarmFilter
            {
                Page = criterias.Page,
                PageSize = criterias.PageSize.HasValue && criterias.PageSize < _pagerOptions.PageSize ? criterias.PageSize.Value : _pagerOptions.PageSize,
                Keyword = criterias.Search,
                CreatedById = userId,
                CanGetInactived = currentUserId == userId
            };

            try
            {
                var farmPageList = await _farmAppService.GetAsync(filterRequest);
                var farms = await MapFarmsResultToModelAsync(farmPageList.Collections);

                var farmPage = new FarmPageListModel(farms)
                {
                    Filter = criterias,
                    TotalPage = farmPageList.TotalPage,
                    TotalResult = farmPageList.TotalResult
                };

                return farmPage;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<FarmPageListModel> GetFarmsAsync(FarmFilterModel criterias)
        {
            if (criterias == null)
            {
                criterias = new FarmFilterModel();
            }

            var filterRequest = new FarmFilter()
            {
                Page = criterias.Page,
                PageSize = criterias.PageSize.HasValue && criterias.PageSize < _pagerOptions.PageSize ? criterias.PageSize.Value : _pagerOptions.PageSize,
                Keyword = criterias.Search
            };

            if (!string.IsNullOrEmpty(criterias.ExclusiveUserIdentityId))
            {
                filterRequest.ExclusiveUserId = await _userManager.DecryptUserIdAsync(criterias.ExclusiveUserIdentityId);
            }

            try
            {
                var farmPageList = await _farmAppService.GetAsync(filterRequest);
                var farms = await MapFarmsResultToModelAsync(farmPageList.Collections);

                var farmPage = new FarmPageListModel(farms)
                {
                    Filter = criterias,
                    TotalPage = farmPageList.TotalPage,
                    TotalResult = farmPageList.TotalResult
                };

                return farmPage;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<FarmModel> GetFarmAsync(FarmIdFilterModel criterias)
        {
            if (criterias == null)
            {
                criterias = new FarmIdFilterModel();
            }

            if (criterias.Id <= 0)
            {
                throw new ArgumentNullException(nameof(criterias.Id));
            }

            try
            {
                var farmResult = await _farmAppService.FindDetailAsync(new IdRequestFilter<long>
                {
                    Id = criterias.Id,
                    CanGetInactived = true
                });

                var farm = await MapFarmResultToModelAsync(farmResult);
                return farm;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<FarmModel> MapFarmResultToModelAsync(FarmResult farmResult)
        {
            var farm = new FarmModel()
            {
                FarmTypeId = farmResult.FarmTypeId,
                FarmTypeName = farmResult.FarmTypeName,
                Description = farmResult.Description,
                Id = farmResult.Id,
                CreatedBy = farmResult.CreatedBy,
                CreatedById = farmResult.CreatedById,
                CreatedDate = farmResult.CreatedDate,
                Name = farmResult.Name,
                Address = farmResult.Address,
                CreatedByPhotoId = farmResult.CreatedByPhotoId,
                Pictures = farmResult.Pictures.Select(y => new PictureResultModel()
                {
                    PictureId = y.Id
                }),
            };

            farm.CreatedByIdentityId = await _userManager.EncryptUserIdAsync(farm.CreatedById);

            return farm;
        }

        private async Task<IList<FarmModel>> MapFarmsResultToModelAsync(IEnumerable<FarmResult> farmResults)
        {
            var farms = farmResults.Select(x => new FarmModel()
            {
                FarmTypeId = x.FarmTypeId,
                FarmTypeName = x.FarmTypeName,
                Description = x.Description,
                Id = x.Id,
                CreatedBy = x.CreatedBy,
                CreatedById = x.CreatedById,
                CreatedDate = x.CreatedDate,
                Name = x.Name,
                Address = x.Address,
                CreatedByPhotoId = x.CreatedByPhotoId,
                Pictures = x.Pictures.Select(y => new PictureResultModel()
                {
                    PictureId = y.Id
                }),
            }).ToList();

            foreach (var farm in farms)
            {
                farm.CreatedByIdentityId = await _userManager.EncryptUserIdAsync(farm.CreatedById);
                if (farm.Description.Length >= 150)
                {
                    farm.Description = $"{farm.Description.Substring(0, 150)}...";
                }
            }

            return farms;
        }
    }
}
