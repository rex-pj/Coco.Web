﻿using System.Collections.Generic;

namespace Camino.Business.Dtos.Identity
{
    public class AuthorizationPolicyUsersDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public IEnumerable<UserDto> AuthorizationPolicyUsers { get; set; }
    }
}