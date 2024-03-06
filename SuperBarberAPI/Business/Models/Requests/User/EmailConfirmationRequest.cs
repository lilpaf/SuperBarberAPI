using System.ComponentModel.DataAnnotations;

namespace Business.Models.Requests.User
{
    public class EmailConfirmationRequest
    {
        [Required]
        public required string Code { get; init; }
    }
}
