﻿using System;
using System.Collections.Generic;
using System.Linq;
using Camino.Core.Contracts.Validations;
using Camino.Shared.Results.Errors;
using Camino.Core.Domain.Identifiers;
using Camino.Shared.Requests.UpdateItems;

namespace Camino.Core.Validations
{
    public class UserInfoItemUpdationValidationStratergy : IValidationStrategy
    {
        public IEnumerable<BaseErrorResult> Errors { get; set; }
        private readonly IValidationStrategyContext _validationStrategyContext;
        public UserInfoItemUpdationValidationStratergy(IValidationStrategyContext validationStrategyContext)
        {
            _validationStrategyContext = validationStrategyContext;
        }

        public bool IsValid<T>(T value)
        {
            var model = value as PartialUpdateRequest;
            var propertyName = model.PropertyName;
            UserInfo userInfo;

            var ignoreCase = StringComparison.InvariantCultureIgnoreCase;

            if (propertyName.Equals(nameof(userInfo.Id), ignoreCase) || propertyName.Equals(nameof(userInfo.User), ignoreCase))
            {
                Errors = GetErrors(new NotSupportedException($"Not support {propertyName}"));
                return false;
            }
            
            if (propertyName.Equals(nameof(userInfo.PhoneNumber), ignoreCase))
            {
                _validationStrategyContext.SetStrategy(new PhoneValidationStrategy());
                bool isValid = (model.Value == null || string.IsNullOrEmpty(model.Value.ToString())) || _validationStrategyContext.Validate(model.Value);

                if (!isValid)
                {
                    Errors = _validationStrategyContext.Errors;
                }
            }
            else if (propertyName.Equals(nameof(userInfo.BirthDate), ignoreCase) && model.Value == null)
            {
                Errors = GetErrors(new NotSupportedException(nameof(userInfo.BirthDate)));
            }

            return Errors == null || !Errors.Any();
        }

        public IEnumerable<BaseErrorResult> GetErrors(Exception exception)
        {
            yield return new BaseErrorResult()
            {
                Message = exception.Message
            };
        }
    }
}
