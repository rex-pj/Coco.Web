﻿using System.Collections.Generic;

namespace Camino.Infrastructure.Identity.Core
{
    public class ApplicationUserRoleAuthorizationPolicy
    {
        public ApplicationUserRoleAuthorizationPolicy()
        {
            Roles = new List<ApplicationRole>();
            AuthorizationPolicies = new List<ApplicationAuthorizationPolicy>();
        }

        public IEnumerable<ApplicationRole> Roles { get; set; }
        public IEnumerable<ApplicationAuthorizationPolicy> AuthorizationPolicies { get; set; }
    }
}
