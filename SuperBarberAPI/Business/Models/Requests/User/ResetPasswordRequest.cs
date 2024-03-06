using Business.Constants;
using Business.Constants.Messages;
using System.ComponentModel.DataAnnotations;

namespace Business.Models.Requests.User
{
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; init; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(DataConstraints.PasswordMaxLength, MinimumLength = DataConstraints.PasswordMinLength)]
        public required string NewPassword { get; init; }

        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.ConfirmationPasswordDoNotMatch))]
        public required string ConfirmPassword { get; init; }

        [Required]
        public required string Code { get; init; }
    }
}
