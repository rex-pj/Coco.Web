﻿using Camino.Core.Domain.Farms;
using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.Products
{
    public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public long CreatedById { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public long UpdatedById { get; set; }
        public int StatusId { get; set; }
        public virtual ICollection<ProductCategoryRelation> ProductCategories { get; set; }
        public virtual ICollection<ProductPicture> ProductPictures { get; set; }
        public virtual ICollection<FarmProduct> ProductFarms { get; set; }
    }
}
