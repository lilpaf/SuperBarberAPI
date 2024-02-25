using System.ComponentModel.DataAnnotations;

namespace Business.Models.Requests
{
    public class LogInRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
