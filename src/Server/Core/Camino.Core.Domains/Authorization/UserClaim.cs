﻿using Camino.Core.Domains.Users;

namespace Camino.Core.Domains.Authorization
{
    public class UserClaim
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public virtual User User { get; set; }
    }
}
