﻿using Microsoft.AspNetCore.Builder;

namespace Module.Web.DashboardManagement.Middlewares
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder ConfigureAppBuilder(this IApplicationBuilder app)
        {
            return app;
        }
    }
}
