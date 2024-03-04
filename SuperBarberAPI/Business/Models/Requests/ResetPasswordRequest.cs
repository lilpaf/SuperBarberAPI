﻿using Business.Constants;
using System.ComponentModel.DataAnnotations;

namespace Business.Models.Requests
{
    public class ResetPasswordRequest
    {
        [Required]
        [Display(Name = AuthenticationConstants.NewPasswordDisplayName)]
        [DataType(DataType.Password)]
        [StringLength(DataConstraints.PasswordMaxLength, MinimumLength = DataConstraints.PasswordMinLength)]
        public string NewPassword { get; init; }

        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = AuthenticationConstants.ConfirmationPasswordDoNotMatch)]
        public string ConfirmPassword { get; init; }

        [Required]
        public string Code { get; init; } = null!;
    }
}
