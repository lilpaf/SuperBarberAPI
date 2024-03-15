using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Entities
{
    public class BarberShopServiceOrder
    {
        public int Id { get; set; }

        public required int BarberShopServiceId { get; set; }

        [ForeignKey(nameof(BarberShopServiceId))]
        public BarberShopService BarberShopService { get; set; } = null!;

        public required Guid OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; } = null!;
    }
}
