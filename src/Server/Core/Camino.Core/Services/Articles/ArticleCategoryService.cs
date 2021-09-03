﻿using Camino.Shared.Requests.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Shared.Results.PageList;
using Camino.Shared.Results.Articles;
using Camino.Shared.Requests.Articles;
using Camino.Core.Contracts.Services.Articles;
using Camino.Core.Contracts.Repositories.Articles;
using Camino.Core.Contracts.Repositories.Users;
using System.Linq;
using Camino.Core.Exceptions;
using Camino.Shared.General;

namespace Camino.Services.Articles
{
    public class ArticleCategoryService : IArticleCategoryService
    {
        private readonly IArticleCategoryRepository _articleCategoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IArticleCategoryStatusRepository _articleCategoryStatusRepository;

        public ArticleCategoryService(IArticleCategoryRepository articleCategoryRepository, IUserRepository userRepository,
            IArticleRepository articleRepository, IArticleCategoryStatusRepository articleCategoryStatusRepository)
        {
            _articleCategoryRepository = articleCategoryRepository;
            _userRepository = userRepository;
            _articleRepository = articleRepository;
            _articleCategoryStatusRepository = articleCategoryStatusRepository;
        }

        #region get
        public async Task<ArticleCategoryResult> FindAsync(IdRequestFilter<int> filter)
        {
            var category = await _articleCategoryRepository.FindAsync(filter);
            if (category == null)
            {
                return null;
            }

            var createdByUser = await _userRepository.FindByIdAsync(category.CreatedById);
            var updatedByUser = await _userRepository.FindByIdAsync(category.UpdatedById);

            category.CreatedBy = createdByUser.DisplayName;
            category.UpdatedBy = updatedByUser.DisplayName;

            return category;
        }

        public async Task<ArticleCategoryResult> FindByNameAsync(string name)
        {
            return await _articleCategoryRepository.FindByNameAsync(name);
        }

        public async Task<BasePageList<ArticleCategoryResult>> GetAsync(ArticleCategoryFilter filter)
        {
            var categoryPageList = await _articleCategoryRepository.GetAsync(filter);

            var createdByIds = categoryPageList.Collections.Select(x => x.CreatedById).ToArray();
            var updatedByIds = categoryPageList.Collections.Select(x => x.UpdatedById).ToArray();

            var createdByUsers = await _userRepository.GetNameByIdsAsync(createdByIds);
            var updatedByUsers = await _userRepository.GetNameByIdsAsync(updatedByIds);

            foreach (var category in categoryPageList.Collections)
            {
                var createdBy = createdByUsers.FirstOrDefault(x => x.Id == category.CreatedById);
                category.CreatedBy = createdBy.DisplayName;

                var updatedBy = updatedByUsers.FirstOrDefault(x => x.Id == category.CreatedById);
                category.UpdatedBy = updatedBy.DisplayName;
            }

            return categoryPageList;
        }

        public IList<ArticleCategoryResult> SearchParents(IdRequestFilter<int?> idRequestFilter, BaseFilter filter)
        {
            return _articleCategoryRepository.SearchParents(idRequestFilter, filter);
        }

        public IList<ArticleCategoryResult> Search(IdRequestFilter<int?> idRequestFilter, BaseFilter filter)
        {
            return _articleCategoryRepository.Search(idRequestFilter, filter);
        }
        #endregion

        #region CRUD
        public async Task<int> CreateAsync(ArticleCategoryModifyRequest category)
        {
            return await _articleCategoryRepository.CreateAsync(category);
        }

        public async Task<bool> UpdateAsync(ArticleCategoryModifyRequest category)
        {
            return await _articleCategoryRepository.UpdateAsync(category);
        }

        public async Task<bool> ActiveAsync(ArticleCategoryModifyRequest request)
        {
            return await _articleCategoryRepository.ActiveAsync(request);
        }

        public async Task<bool> DeactivateAsync(ArticleCategoryModifyRequest request)
        {
            return await _articleCategoryRepository.DeactivateAsync(request);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var articles = await _articleRepository.GetArticleByCategoryIdAsync(new IdRequestFilter<int>
            {
                Id = id,
                CanGetDeleted = true,
                CanGetInactived = true
            });

            if (articles.Any())
            {
                throw new CaminoApplicationException($"Some {nameof(articles)} belong to this category need to be deleted or move to another category");
            }

            return await _articleCategoryRepository.DeleteAsync(id);
        }
        #endregion

        #region category status
        public IList<SelectOption> SearchStatus(IdRequestFilter<int?> filter, string search = "")
        {
            return _articleCategoryStatusRepository.Search(filter, search);
        }
        #endregion
    }
}
