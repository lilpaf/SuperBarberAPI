using Business.Models.Dtos;
using Common.Constants.Resourses;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Business.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class HoursFormatAttribute : ValidationAttribute
    {
        private readonly Regex _regex = new(@"\b(?:0[0-9]|1[0-9]|2[0-3]):(?:00|30)\b", RegexOptions.Compiled);

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DayHoursDto singleHours)
            {
                if (!IsValidFormat(singleHours))
                {
                    return new ValidationResult(Messages.InvalidDateOrHourFormat);
                }

                return ValidationResult.Success;
            }


            if (value is Dictionary<string, DayHoursDto> workingWeekHours)
            {
                foreach (var hours in workingWeekHours.Values)
                {
                    if (!IsValidFormat(hours))
                    {
                        return new ValidationResult(Messages.InvalidDateOrHourFormat);
                    }
                }

                return ValidationResult.Success;
            }

            return new ValidationResult(Messages.InvalidDateOrHourFormat);
        }

        private bool IsValidFormat(DayHoursDto hours)
        {
            string? openingTime = hours.OpeningTime;
            string? closingTime = hours.ClosingTime;

            // If both are null, validation succeeds
            if (openingTime is null && closingTime is null)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(openingTime) || string.IsNullOrWhiteSpace(closingTime))
            {
                return false;
            }

            if (!_regex.IsMatch(openingTime) || !_regex.IsMatch(closingTime))
            {
                return false;
            }

            return true;
        }
    }
}
