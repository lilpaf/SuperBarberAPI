using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Entities
{
    public class BarberShopService
    {
        public int Id { get; set; }

        public required int BarberShopId { get; set; }

        [ForeignKey(nameof(BarberShopId))]
        public BarberShop BarberShop { get; set; } = null!;

        public required int ServiceId { get; set; }

        [ForeignKey(nameof(ServiceId))]
        public Service Service { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public required decimal Price { get; set; }

        public required TimeSpan TimeToExecute { get; set; }

        public ICollection<BarberShopServiceOrder> Orders { get; set; } = new HashSet<BarberShopServiceOrder>();
    }
}
