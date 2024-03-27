using Business.Attributes;
using Business.Models.Dtos;
using Common.Constants;
using Common.Constants.Resourses;
using System.ComponentModel.DataAnnotations;

namespace Business.Models.Requests.BarberShop
{
    public class RegisterBarberShopRequest
    {
        [Required]
        [StringLength(DataConstraints.ShopNameMaxLength, MinimumLength = DataConstraints.DefaultMinLength)]
        public required string Name { get; init; }

        [Required]
        public required string City { get; init; }

        public required string? Neighborhood { get; init; }

        [Required]
        [StringLength(DataConstraints.AddressMaxLength, MinimumLength = DataConstraints.DefaultMinLength)]
        public required string Address { get; init; }
        
        [StringLength(DataConstraints.AboutMaxLength)]
        public required string? About { get; init; }

        [Required]
        [Length(DataConstraints.WorkingDaysHoursLength, DataConstraints.WorkingDaysHoursLength, 
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = nameof(Messages.InvalidAndDateHourFormat))]
        [ValidWorkingWeekHours]
        public required Dictionary<string, DayHoursDto> WorkingDaysHours { get; init; }

        //ToDo fix it
        //[Required]
        //[Display(Name = "Image")]
        //public required IFormFile ImageFile { get; init; }
    }
}
