﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Module.Dashboard.WebAdmin.Extensions.DependencyInjection;
using Camino.Infrastructure.Modularity;
using Module.Dashboard.WebAdmin.Middlewares;

namespace Module.Dashboard.WebAdmin
{
    public class Startup : ModuleStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureFileServices();
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ConfigureAppBuilder();
        }
    }
}
