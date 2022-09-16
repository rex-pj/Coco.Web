﻿namespace Camino.Application.Contracts.AppServices.Farms.Dtos
{
    public class FarmTypeResult
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime UpdatedDate { get; set; }

        public long UpdatedById { get; set; }
        public string UpdatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public long CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public int StatusId { get; set; }
    }
}
