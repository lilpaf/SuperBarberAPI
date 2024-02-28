using System.ComponentModel.DataAnnotations;

namespace Business.Models.Requests
{
    public class UserLoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; init; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; init; } = null!;
    }
}
