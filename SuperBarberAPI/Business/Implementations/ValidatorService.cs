using Business.Constants;
using Business.Constants.Messages;
using Business.Interfaces;
using Business.Models.Exceptions;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Business.Implementations
{
    public class ValidatorService : IValidatorService
    {
        private readonly ILogger<ValidatorService> _logger;
        
        public ValidatorService(ILogger<ValidatorService> logger)
        {
            _logger = logger;
        }

        public void Validate<T>(T model)
        {
            if (model is null)
            {
                _logger.LogError("Validation object can not be null");
                throw new InvalidValidationObjectException(Messages.InvalidValidationObject);
            }

            var context = new ValidationContext(model);
            var errors = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, context, errors, true))
            {
                string[] errorMessages = errors.Select(x => x.ErrorMessage).ToArray();
                string message = string.Join(ErrorConstants.ErrorDelimiter, errorMessages);

                throw new InvalidModelStateException(message, errorMessages);
            }
        }
    }
}
