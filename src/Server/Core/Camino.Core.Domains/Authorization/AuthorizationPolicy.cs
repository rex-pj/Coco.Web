﻿using Camino.Core.Domains.Users;
using System.ComponentModel.DataAnnotations;

namespace Camino.Core.Domains.Authorization
{
    public class AuthorizationPolicy
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public long CreatedById { get; set; }
        [Required]
        public DateTime UpdatedDate { get; set; }
        [Required]
        public long UpdatedById { get; set; }
        public virtual User CreatedBy { get; set; }
        public virtual User UpdatedBy { get; set; }
        public virtual ICollection<UserAuthorizationPolicy> AuthorizationPolicyUsers { get; set; }
        public virtual ICollection<RoleAuthorizationPolicy> AuthorizationPolicyRoles { get; set; }
    }
}
