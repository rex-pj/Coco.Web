﻿using Camino.Shared.Requests.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Shared.Results.PageList;
using Camino.Core.Contracts.Services.Authorization;
using Camino.Shared.Requests.Authorization;
using Camino.Shared.Results.Authorization;
using Camino.Core.Contracts.Repositories.Authorization;
using Camino.Core.Contracts.DependencyInjection;

namespace Camino.Services.Authorization
{
    public class RoleService : IRoleService, IScopedDependency
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        #region CRUD
        public async Task<long> CreateAsync(RoleModifyRequest request)
        {
            return await _roleRepository.CreateAsync(request);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _roleRepository.DeleteAsync(id);
        }

        public async Task<RoleResult> FindAsync(long id)
        {
            return await _roleRepository.FindAsync(id);
        }

        public List<RoleResult> Search(BaseFilter filter, List<long> currentRoleIds = null)
        {
            return _roleRepository.Search(filter, currentRoleIds);
        }

        public RoleResult FindByName(string name)
        {
            return _roleRepository.FindByName(name);
        }

        public async Task<RoleResult> FindByNameAsync(string name)
        {
            return await _roleRepository.FindByNameAsync(name);
        }

        public async Task<BasePageList<RoleResult>> GetAsync(RoleFilter filter)
        {
            return await _roleRepository.GetAsync(filter);
        }

        public async Task<bool> UpdateAsync(RoleModifyRequest request)
        {
            return await _roleRepository.UpdateAsync(request);
        }

        public async Task<RoleResult> GetByNameAsync(string name)
        {
            return await _roleRepository.GetByNameAsync(name);
        }
        #endregion
    }
}
