﻿using Camino.Shared.Results.Farms;
using Camino.Shared.Requests.Filters;
using Camino.Shared.Results.PageList;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Shared.Requests.Farms;

namespace Camino.Core.Contracts.Repositories.Farms
{
    public interface IFarmRepository
    {
        Task<FarmResult> FindAsync(IdRequestFilter<long> filter);
        Task<FarmResult> FindDetailAsync(IdRequestFilter<long> filter);
        FarmResult FindByName(string name);
        Task<BasePageList<FarmResult>> GetAsync(FarmFilter filter);
        Task<long> CreateAsync(FarmModifyRequest request);
        Task<IList<FarmResult>> GetFarmByTypeIdAsync(IdRequestFilter<int> typeIdFilter);
        Task<bool> UpdateAsync(FarmModifyRequest request);
        Task<IList<FarmResult>> SelectAsync(SelectFilter filter, int page, int pageSize);
        Task<bool> DeleteAsync(long id);
        Task<bool> SoftDeleteAsync(FarmModifyRequest request);
        Task<bool> DeactivateAsync(FarmModifyRequest request);
        Task<bool> ActiveAsync(FarmModifyRequest request);
    }
}
