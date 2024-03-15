using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Persistence.Entities
{
    public class BarberShop
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public required int CityId { get; set; }

        [ForeignKey(nameof(CityId))]
        public City City { get; set; } = null!;

        public int? NeighborhoodId { get; set; }

        [ForeignKey(nameof(NeighborhoodId))]
        public required Neighborhood? Neighborhood { get; set; }

        [Required]
        public required string Address { get; set; }
        
        public required string? About { get; set; }

        //ToDo fix it
        //public required string ImageName { get; set; } = null!;

        public required bool IsPublic { get; set; }

        public required bool IsDeleted { get; set; }

        public DateTime? DeleteDate { get; set; }

        public required double AverageRating { get; set; }

        public required ICollection<BarberShopWorkingDay> BarberShopWorkingDays { get; set; } = new HashSet<BarberShopWorkingDay>();
        
        public ICollection<BarberShopService> Services { get; set; } = new HashSet<BarberShopService>();

        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();

        //ToDo make it required
        public ICollection<BarberShopBarber> Barbers { get; set; } = new HashSet<BarberShopBarber>();
        
        public ICollection<BarberShopRating> Ratings { get; set; } = new HashSet<BarberShopRating>();
    }
}
