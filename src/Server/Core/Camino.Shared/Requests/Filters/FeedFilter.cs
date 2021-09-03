﻿using Camino.Shared.Enums;
using System;

namespace Camino.Shared.Requests.Filters
{
    public class FeedFilter : BaseFilter
    {
        public DateTimeOffset? CreatedDateFrom { get; set; }
        public DateTimeOffset? CreatedDateTo { get; set; }
        public long? CreatedById { get; set; }
        public bool CanGetInactived { get; set; }
        public bool CanGetDeleted { get; set; }
        public FeedFilterType? FilterType { get; set; }
    }
}
