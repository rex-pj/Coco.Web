﻿using Camino.Framework.Infrastructure.Extensions;
using Camino.Shared.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using Camino.Infrastructure.Infrastructure.Extensions;

namespace Camino.ApiHost.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureApiHostServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication();
            services.AddApplicationServices(configuration);

            services.AddInfrastructureServices();
            services.AddHttpContextAccessor()
                .ConfigureCorsServices(services.BuildServiceProvider());

            services.AddControllers()
                .AddNewtonsoftJson()
                .AddModular();
            services.AddAutoMappingModular();
            services.AddGraphQlModular();
            return services;
        }

        public static IServiceCollection ConfigureCorsServices(this IServiceCollection services, IServiceProvider serviceProvider)
        {
            var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;
            services.AddCors(options =>
            {
                options.AddPolicy(appSettings.MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins(appSettings.AllowOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            return services;
        }
    }
}
