using System.ComponentModel.DataAnnotations;

namespace Business.Models.Requests.User
{
    public class ResetPasswordEmailRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; init; }
    }
}
