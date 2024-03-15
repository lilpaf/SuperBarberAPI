using System.ComponentModel.DataAnnotations;

namespace Persistence.Entities
{
    public class City
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public ICollection<BarberShop> BarberShops { get; set; } = new HashSet<BarberShop>();
        
        public ICollection<Neighborhood> Neighborhoods { get; set; } = new HashSet<Neighborhood>();
    }
}
