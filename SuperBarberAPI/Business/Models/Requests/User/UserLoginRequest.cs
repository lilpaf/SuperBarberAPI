using System.ComponentModel.DataAnnotations;

namespace Business.Models.Requests.User
{
    public class UserLoginRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; init; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; init; }
    }
}
