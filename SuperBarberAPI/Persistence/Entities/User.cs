using Microsoft.AspNetCore.Identity;

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
    }
}
