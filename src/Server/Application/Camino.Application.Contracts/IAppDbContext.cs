﻿using Camino.Core.Domains;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Camino.Application.Contracts
{
    public interface IAppDbContext : IDbContext
    {
        //
        // Summary:
        //     Provides access to database related information and operations for this context.
        DatabaseFacade Database { get; }
    }
}