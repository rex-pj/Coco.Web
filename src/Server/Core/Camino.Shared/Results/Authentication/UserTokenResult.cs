﻿using System;

namespace Camino.Shared.Results.Authentication
{
    public class UserTokenResult
    {
        public long Id { get; set; }
        public virtual string LoginProvider { get; set; }
        public virtual string Name { get; set; }
        public virtual long UserId { get; set; }
        public virtual string Value { get; set; }
        public DateTimeOffset ExpiryTime { get; set; }
    }
}
