﻿using Camino.Core.Domains.Users;
using System.ComponentModel.DataAnnotations;

namespace Camino.Core.Domains.Authorization
{
    public class UserAuthorizationPolicy
    {
        public long UserId { get; set; }
        public long AuthorizationPolicyId { get; set; }
        [Required]
        public DateTime GrantedDate { get; set; }
        [Required]
        public long GrantedById { get; set; }
        [Required]
        public bool IsGranted { get; set; }
        public virtual User User { get; set; }
        public virtual User GrantedBy { get; set; }
        public virtual AuthorizationPolicy AuthorizationPolicy { get; set; }
    }
}
