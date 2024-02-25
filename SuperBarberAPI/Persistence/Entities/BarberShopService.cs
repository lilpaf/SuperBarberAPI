using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Entities
{
    public class BarberShopService
    {
        public int BarberShopId { get; set; }

        [ForeignKey(nameof(BarberShopId))]
        public BarberShop BarberShop { get; set; }

        public int ServiceId { get; set; }

        [ForeignKey(nameof(ServiceId))]
        public Service Service { get; set; }

        public decimal Price { get; set; }
    }
}
