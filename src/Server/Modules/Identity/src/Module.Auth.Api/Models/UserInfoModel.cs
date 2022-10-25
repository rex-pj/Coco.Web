﻿using System;

namespace Module.Auth.Api.Models
{
    public class UserInfoModel
    {
        public string UserIdentityId { get; set; }
        public string Email { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string DisplayName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? GenderId { get; set; }
        public string? GenderLabel { get; set; }
        public int StatusId { get; set; }
        public string StatusLabel { get; set; }
        public short? CountryId { get; set; }
        public string? CountryCode { get; set; }
        public string? CountryName { get; set; }
        public long? AvatarId { get; set; }
        public bool CanEdit { get; set; }
    }
}
