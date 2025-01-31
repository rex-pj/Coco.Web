﻿using Camino.Application.Contracts;
using Camino.Infrastructure.AspNetCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Module.Farm.Api.GraphQL.Resolvers.Contracts
{
    public interface IFarmTypeResolver
    {
        Task<IEnumerable<SelectOption>> GetFarmTypesAsync(BaseSelectFilterModel criterias);
    }
}
