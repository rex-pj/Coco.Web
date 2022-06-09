﻿namespace Camino.Application.Contracts.AppServices.Authentication.Dtos
{
    public class UserPasswordUpdateRequest
    {
        public long UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
