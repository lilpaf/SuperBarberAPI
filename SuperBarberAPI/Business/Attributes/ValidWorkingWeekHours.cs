using Business.Models.Dtos;
using Common.Constants.Resourses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Business.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidWorkingWeekHours : ValidationAttribute
    {
        private readonly Regex _regex;

        public ValidWorkingWeekHours()
        {
            _regex = new(@"\b(?:0[0-9]|1[0-9]|2[0-3]):(?:00|30)\b");
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            ILogger<ValidWorkingWeekHours> logger = validationContext.GetRequiredService<ILogger<ValidWorkingWeekHours>>();

            if (value is not Dictionary<string, DayHoursDto> workingWeekHours)
            {
                logger.LogError("Validation value is not of the correct type");
                return new ValidationResult(Messages.InvalidAndDateHourFormat);
            }

            if (workingWeekHours.Keys.Any(day => !Enum.GetNames<DayOfWeek>().Contains(day)))
            {
                logger.LogError("Working week day is not contained in day of week enum");
                return new ValidationResult(Messages.InvalidAndDateHourFormat);
            }
            
            foreach (var hours in workingWeekHours.Values)
            {
                string? openingTime = hours.OpeningTime;
                string? closingTime = hours.ClosingTime;

                if (openingTime is null && closingTime is null)
                {
                    continue;
                }
                else if (openingTime is null || closingTime is null)
                {
                    logger.LogError("Opening time is null but closing time is not null, or the opposite");
                    return new ValidationResult(Messages.InvalidAndDateHourFormat);
                }

                if (!_regex.IsMatch(openingTime) || !_regex.IsMatch(closingTime))
                {
                    logger.LogError("Opening time did not match regex or closing time did not match regex");
                    return new ValidationResult(Messages.InvalidAndDateHourFormat);
                }
            }

            return ValidationResult.Success!;
        }
    }
}