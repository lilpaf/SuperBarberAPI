using System.ComponentModel.DataAnnotations;

namespace Business.Models.Requests
{
    public class UserLoginRequest
    {
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
