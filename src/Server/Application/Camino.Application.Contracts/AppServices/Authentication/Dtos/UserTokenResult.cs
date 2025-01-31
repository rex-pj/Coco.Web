﻿using System;

namespace Camino.Application.Contracts.AppServices.Authentication.Dtos
{
    public class UserTokenResult
    {
        public long Id { get; set; }
        public virtual string LoginProvider { get; set; }
        public virtual string Name { get; set; }
        public virtual long UserId { get; set; }
        public virtual string Value { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}
