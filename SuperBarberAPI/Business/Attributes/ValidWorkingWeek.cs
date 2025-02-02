using Business.Models.Dtos;
using Common.Constants.Resourses;
using System.ComponentModel.DataAnnotations;

namespace Business.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidWorkingWeek : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not Dictionary<string, DayHoursDto> workingWeekHours)
            {
                return new ValidationResult(Messages.InvalidDateOrHourFormat);
            }

            var validDays = Enum.GetNames<DayOfWeek>().ToHashSet();

            if (workingWeekHours.Keys.Any(day => !validDays.Contains(day)))
            {
                return new ValidationResult(Messages.InvalidDateOrHourFormat);
            }

            return ValidationResult.Success;
        }

    }
}