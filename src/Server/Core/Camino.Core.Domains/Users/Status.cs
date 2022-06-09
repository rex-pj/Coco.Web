﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Camino.Core.Domains.Users
{
    public class Status
    {
        public Status()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
