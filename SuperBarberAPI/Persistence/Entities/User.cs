using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Persistence.Entities
{
    public class User : IdentityUser
    {
        public required string? FirstName { get; set; }

        public required string? LastName { get; set; }

        public required bool IsDeleted { get; set; }

        public required DateTime? DeleteDate { get; set; }

        public required double AverageRating { get; set; }

        public ICollection<Order> Reservations { get; set; } = new HashSet<Order>();

        public ICollection<UserRating> Ratings { get; set; } = new HashSet<UserRating>();

        public ICollection<UserRefreshToken> RefreshTokens { get; set; } = new HashSet<UserRefreshToken>();
    }
}
