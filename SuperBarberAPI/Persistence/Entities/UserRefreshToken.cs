using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Entities
{
    public class UserRefreshToken
    {
        public int Id { get; set; }

        [Required]
        public required string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [Required]
        public required string Token { get; set; }

        [Required]
        public required string JwtId { get; set; }

        public required bool IsUsed { get; set; }

        public required bool IsRevoked { get; set; }

        public required DateTime CreatedDate { get; set; }

        public required DateTime ExpiryDate { get; set; }
    }
}
