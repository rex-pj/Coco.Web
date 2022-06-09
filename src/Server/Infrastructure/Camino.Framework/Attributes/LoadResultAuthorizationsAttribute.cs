﻿using Camino.Framework.Models;
using Camino.Infrastructure.Identity.Core;
using Camino.Infrastructure.Identity.Interfaces;
using Camino.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Framework.Attributes
{
    public class LoadResultAuthorizationsAttribute : ResultFilterAttribute
    {
        private PolicyMethods[] _policyMethods;
        private readonly string _moduleName;

        public LoadResultAuthorizationsAttribute(string moduleName, params PolicyMethods[] policyMethods)
        {
            _moduleName = moduleName;
            _policyMethods = policyMethods;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is null)
            {
                await next();
            }

            var isViewResult = context.Result is ViewResult;
            var isPartialViewResult = context.Result is PartialViewResult;
            if (!isViewResult && !isPartialViewResult)
            {
                await next();
            }

            var viewModel = isViewResult ? (context.Result as ViewResult).Model
                : (context.Result as PartialViewResult).Model;

            if (!(viewModel is BaseModel))
            {
                await next();
            }

            var httpContext = context.HttpContext;
            if (_moduleName != null && !_policyMethods.Any())
            {
                _policyMethods = Enum.GetValues(typeof(PolicyMethods)).Cast<PolicyMethods>().ToArray();
            }

            var policies = _policyMethods.Select(x => $"{x}{_moduleName}").ToArray();
            var requestServices = httpContext.RequestServices;
            var userManager = requestServices.GetRequiredService<IUserManager<ApplicationUser>>();
            var numberOfPolicies = policies.Length;
            var model = viewModel as BaseModel;
            for (int i = 0; i < numberOfPolicies; i++)
            {
                var policyMethod = _policyMethods[i].ToString();
                var propertyInfo = model.GetType().GetProperty(policyMethod);

                var hasPolicy = await userManager.HasPolicyAsync(httpContext.User, policies[i]);
                propertyInfo.SetValue(model, hasPolicy, null);
            }

            await next();
        }
    }
}
