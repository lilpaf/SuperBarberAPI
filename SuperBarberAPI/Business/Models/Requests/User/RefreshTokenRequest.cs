using System.ComponentModel.DataAnnotations;

namespace Business.Models.Requests.User
{
    public class RefreshTokenRequest
    {
        [Required]
        public required string AccessToken { get; init; }
    }
}
