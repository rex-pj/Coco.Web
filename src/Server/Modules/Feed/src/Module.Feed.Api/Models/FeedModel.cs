﻿using System;

namespace Module.Feed.Api.Models
{
    public class FeedModel
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PictureId { get; set; }
        public long CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public string? Address { get; set; }
        public decimal? Price { get; set; }
        public int FeedType { get; set; }
        public long CreatedByPhotoId { get; set; }
        public string CreatedByIdentityId { get; set; }
    }
}
