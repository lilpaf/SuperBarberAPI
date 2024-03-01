using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Entities
{
    public class BarberShopService
    {
        public int BarberShopId { get; set; }

        [ForeignKey(nameof(BarberShopId))]
        public BarberShop BarberShop { get; set; } = null!;

        public int ServiceId { get; set; }

        [ForeignKey(nameof(ServiceId))]
        public Service Service { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
