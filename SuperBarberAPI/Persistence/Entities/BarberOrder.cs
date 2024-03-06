using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Entities
{
    public class BarberOrder
    {
        public required Guid OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; } = null!;

        public required int BarberId { get; set; }

        [ForeignKey(nameof(BarberId))]
        public Barber Barber { get; set; } = null!;
    }
}
