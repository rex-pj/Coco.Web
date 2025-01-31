﻿using Camino.Application.Contracts.AppServices.Users.Dtos;
using Camino.Core.Validators;
using Camino.Shared.Exceptions;
using Camino.Shared.Utils;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Camino.Application.Validators
{
    public class UserCoverValidator : BaseValidator<UserPhotoUpdateRequest, bool>
    {
        public override bool IsValid(UserPhotoUpdateRequest data)
        {
            if (data.FileData == null || data.FileData.Length == 0)
            {
                Errors = GetErrors(new PhotoSizeInvalidException(nameof(data.FileData))).ToList();
            }

            var image = ImageUtils.FileDataToImage(data.FileData);
            if (image.Width < 1000 || image.Height < 300)
            {
                Errors = GetErrors(new PhotoSizeInvalidException(1000, 300)).ToList();
            }

            return Errors == null || !Errors.Any();
        }

        public override IEnumerable<ValidatorErrorResult> GetErrors(Exception e)
        {
            yield return new ValidatorErrorResult
            {
                Message = e.Message
            };
        }
    }
}
