﻿using Microsoft.Extensions.DependencyInjection;

namespace Module.Web.IdentityManagement.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureFileServices(this IServiceCollection services)
        {
            return services;
        }
    }
}
