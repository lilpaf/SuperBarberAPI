using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Persistence.Entities
{
    public class BarberShop
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public int CityId { get; set; }

        [ForeignKey(nameof(CityId))]
        public City City { get; set; } = null!;

        public int DistrictId { get; set; }

        [ForeignKey(nameof(DistrictId))]
        public District District { get; set; } = null!;

        [Required]
        public string Street { get; set; } = null!;

        public TimeSpan StartHour { get; set; }

        public TimeSpan FinishHour { get; set; }

        public string? ImageName { get; set; }

        public bool IsPublic { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeleteDate { get; set; }

        public ICollection<BarberShopService> Services { get; set; } = new HashSet<BarberShopService>();

        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();

        public ICollection<BarberShopBarber> Barbers { get; set; } = new HashSet<BarberShopBarber>();
    }
}
