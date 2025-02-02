using Common.Constants.Resourses;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Business.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class DateFormatAttribute : ValidationAttribute
    {
        private readonly string _dateFormat;

        public DateFormatAttribute(string dateFormat)
        {
            _dateFormat = dateFormat;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string dateString)
            {
                if (DateTime.TryParseExact(dateString, _dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(Messages.InvalidDateOrHourFormat);
        }
    }
}
