using Business.Constants;
using System.ComponentModel.DataAnnotations;

namespace Business.Models.Requests.User
{
    public class UserRegisterRequest
    {
        [Required]
        [StringLength(DataConstraints.FirstNameMaxLength, MinimumLength = DataConstraints.DefaultMinLength)]
        public required string FirstName { get; init; }

        [Required]
        [StringLength(DataConstraints.LastNameMaxLength, MinimumLength = DataConstraints.DefaultMinLength)]
        public required string LastName { get; init; }

        [Required]
        [StringLength(DataConstraints.EmailMaxLength, MinimumLength = DataConstraints.EmailMinLength)]
        [EmailAddress]
        public required string Email { get; init; }

        //ToDo check if phone works
        [Required]
        [Phone]
        //[RegularExpression(@"^\+?\d+[ |-]?\d+[ |-]?\d{3}[ |-]?\d{4}$")]
        [StringLength(DataConstraints.PhoneNumberMaxLength, MinimumLength = DataConstraints.PhoneNumberMinLength)]
        public required string PhoneNumber { get; init; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(DataConstraints.PasswordMaxLength, MinimumLength = DataConstraints.PasswordMinLength)]
        public required string Password { get; init; }
    }
}
