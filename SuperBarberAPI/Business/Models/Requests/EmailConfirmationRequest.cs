using System.ComponentModel.DataAnnotations;

namespace Business.Models.Requests
{
    public class EmailConfirmationRequest
    {
        [Required]
        public string UserId { get; init; } = null!;

        [Required]
        public string Code { get; init; } = null!;
    }
}
