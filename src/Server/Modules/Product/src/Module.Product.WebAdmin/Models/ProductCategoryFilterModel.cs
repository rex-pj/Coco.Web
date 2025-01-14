﻿using Camino.Infrastructure.AspNetCore.Models;
using System;

namespace Module.Product.WebAdmin.Models
{
    public class ProductCategoryFilterModel : BaseFilterModel
    {
        public DateTime? CreatedDateFrom { get; set; }
        public DateTime? CreatedDateTo { get; set; }
        public long? CreatedById { get; set; }
        public long? UpdatedById { get; set; }
        public int? StatusId { get; set; }
    }
}
