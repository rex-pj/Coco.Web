﻿using Camino.Framework.Models;
using System;
using System.Collections.Generic;

namespace Module.Api.Farm.Models
{
    public class FarmModel
    {
        public FarmModel()
        {
            Pictures = new List<PictureResultModel>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long UpdatedById { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByPhotoCode { get; set; }
        public string CreatedByIdentityId { get; set; }
        public long FarmTypeId { get; set; }
        public string FarmTypeName { get; set; }
        public string Address { get; set; }
        public IEnumerable<PictureResultModel> Pictures { get; set; }
    }
}
