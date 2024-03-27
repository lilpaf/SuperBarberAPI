using System.ComponentModel.DataAnnotations;
using Common.Constants;
using Business.Attributes;
using Business.Models.Dtos;

namespace Business.Models.Requests.BarberShop
{
    public class UpdateBarberShopRequest
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
        [ValidWorkingWeekHours]
        public required Dictionary<string, DayHoursDto> WorkingDaysHours { get; init; }

        //ToDo fix it
        //[Required]
        //[Display(Name = "Image")]
        //public requiredIFormFile ImageFile { get; init; }
    }
}
