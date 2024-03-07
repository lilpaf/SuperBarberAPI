using Business.Constants;
using Business.Constants.Messages;
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

        [Required]
        [RegularExpression(@"\b(?:0[0-9]|1[0-9]|2[0-3]):(?:00|30)\b", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.InvalidHourFormat))]
        public required string StartHour { get; init; }

        [Required]
        [RegularExpression(@"\b(?:0[0-9]|1[0-9]|2[0-3]):(?:00|30)\b", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.InvalidHourFormat))]
        public required string FinishHour { get; init; }

        //ToDo fix it
        //[Required]
        //[Display(Name = "Image")]
        //public IFormFile ImageFile { get; init; }
    }
}
