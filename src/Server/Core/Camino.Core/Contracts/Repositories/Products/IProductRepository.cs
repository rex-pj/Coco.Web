﻿using Camino.Shared.Requests.Filters;
using System.Threading.Tasks;
using Camino.Shared.Results.PageList;
using Camino.Shared.Results.Products;
using System.Collections.Generic;
using Camino.Shared.Requests.Products;

namespace Camino.Core.Contracts.Repositories.Products
{
    public interface IProductRepository
    {
        Task<long> CreateAsync(ProductModifyRequest request);
        Task<ProductResult> FindAsync(IdRequestFilter<long> filter);
        Task<ProductResult> FindDetailAsync(IdRequestFilter<long> filter);
        ProductResult FindByName(string name);
        Task<bool> UpdateAsync(ProductModifyRequest request);
        Task<BasePageList<ProductResult>> GetAsync(ProductFilter filter);
        Task<IList<ProductResult>> GetRelevantsAsync(long id, ProductFilter filter);
        Task<bool> DeleteAsync(long id);
        Task<bool> SoftDeleteAsync(ProductModifyRequest request);
        Task<bool> DeactiveAsync(ProductModifyRequest request);
        Task<bool> ActiveAsync(ProductModifyRequest request);
        Task<IList<ProductResult>> GetProductByCategoryIdAsync(IdRequestFilter<int> categoryIdFilter);
    }
}