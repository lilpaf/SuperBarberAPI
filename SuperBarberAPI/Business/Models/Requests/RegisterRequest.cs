using Business.Constants;
using System.ComponentModel.DataAnnotations;

namespace Business.Models.Requests
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(DataConstraints.FirstNameMaxLength, MinimumLength = DataConstraints.DefaultMinLength)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(DataConstraints.LastNameMaxLength, MinimumLength = DataConstraints.DefaultMinLength)]
        public string LastName { get; set; }

        [Required]
        [StringLength(DataConstraints.EmailMaxLength, MinimumLength = DataConstraints.EmailMinLength)]
        [EmailAddress]
        public string Email { get; set; }

        //ToDo check if phone works
        [Required]
        [Phone]
        //[RegularExpression(@"^\+?\d+[ |-]?\d+[ |-]?\d{3}[ |-]?\d{4}$")]
        [StringLength(DataConstraints.PhoneNumberMaxLength, MinimumLength = DataConstraints.PhoneNumberMinLength)]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(DataConstraints.PasswordMaxLength, MinimumLength = DataConstraints.PasswordMinLength)]
        public string Password { get; set; }
    }
}
