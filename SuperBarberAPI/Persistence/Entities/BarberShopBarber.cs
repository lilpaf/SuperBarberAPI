using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Entities
{
    public class BarberShopBarber
    {
        public int Id { get; set; }

        public required int BarberShopId { get; set; }

        [ForeignKey(nameof(BarberShopId))]
        public BarberShop BarberShop { get; set; } = null!;

        public required int BarberId { get; set; }

        [ForeignKey(nameof(BarberId))]
        public Barber Barber { get; set; } = null!;

        public required bool IsOwner { get; set; }

        public required bool CanTakeOrders { get; set; }

        public ICollection<BarberShopBarberWeekend> Weekends { get; set; } = new HashSet<BarberShopBarberWeekend>();
    }
}
