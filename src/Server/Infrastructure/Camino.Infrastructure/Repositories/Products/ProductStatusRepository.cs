﻿using Camino.Shared.Requests.Filters;
using System.Collections.Generic;
using System.Linq;
using Camino.Shared.Enums;
using Camino.Core.Utils;
using Camino.Shared.General;
using Camino.Core.Contracts.Repositories.Products;

namespace Camino.Infrastructure.Repositories.Products
{
    public class ProductStatusRepository : IProductStatusRepository
    {
        public ProductStatusRepository()
        {
        }

        public IList<SelectOption> Search(IdRequestFilter<int?> filter, string search = "")
        {
            search = search != null ? search.ToLower() : "";
            var result = new List<SelectOption>();
            if (filter.Id.HasValue)
            {
                var selected = (ProductStatus)filter.Id;
                result = EnumUtil.ToSelectOptions(selected).ToList();
            }
            else
            {
                result = EnumUtil.ToSelectOptions<ProductStatus>().ToList();
            }

            result = result.Where(x => string.IsNullOrEmpty(search) || x.Text.ToLower().Equals(search)).ToList();
            return result;
        }
    }
}
