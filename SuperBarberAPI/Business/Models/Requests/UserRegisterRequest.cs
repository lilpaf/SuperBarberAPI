using Business.Constants;
using System.ComponentModel.DataAnnotations;

namespace Business.Models.Requests
{
    public class UserRegisterRequest
    {
        [Required]
        [StringLength(DataConstraints.FirstNameMaxLength, MinimumLength = DataConstraints.DefaultMinLength)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(DataConstraints.LastNameMaxLength, MinimumLength = DataConstraints.DefaultMinLength)]
        public string LastName { get; set; } = null!;

        [Required]
        [StringLength(DataConstraints.EmailMaxLength, MinimumLength = DataConstraints.EmailMinLength)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        //ToDo check if phone works
        [Required]
        [Phone]
        //[RegularExpression(@"^\+?\d+[ |-]?\d+[ |-]?\d{3}[ |-]?\d{4}$")]
        [StringLength(DataConstraints.PhoneNumberMaxLength, MinimumLength = DataConstraints.PhoneNumberMinLength)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(DataConstraints.PasswordMaxLength, MinimumLength = DataConstraints.PasswordMinLength)]
        public string Password { get; set; } = null!;
    }
}
