using Microsoft.AspNetCore.Identity;

namespace Persistence.Entities
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeleteDate { get; set; }

        public ICollection<Order> Reservations { get; set; } = new HashSet<Order>();
    }
}
