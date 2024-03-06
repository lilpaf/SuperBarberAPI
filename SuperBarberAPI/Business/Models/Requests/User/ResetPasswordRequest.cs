using Business.Constants;
using System.ComponentModel.DataAnnotations;

namespace Business.Models.Requests.User
{
    public class ResetPasswordRequest
    {
        [Required]
        [Display(Name = AuthenticationConstants.NewPasswordDisplayName)]
        [DataType(DataType.Password)]
        [StringLength(DataConstraints.PasswordMaxLength, MinimumLength = DataConstraints.PasswordMinLength)]
        public required string NewPassword { get; init; }

        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = AuthenticationConstants.ConfirmationPasswordDoNotMatch)]
        public required string ConfirmPassword { get; init; }

        [Required]
        public required string Code { get; init; }
    }
}
