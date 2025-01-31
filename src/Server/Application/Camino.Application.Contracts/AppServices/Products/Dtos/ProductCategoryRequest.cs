﻿namespace Camino.Application.Contracts.AppServices.Products.Dtos
{
    public class ProductCategoryRequest
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long UpdatedById { get; set; }
        public long CreatedById { get; set; }
        public long? ParentId { get; set; }
    }
}
