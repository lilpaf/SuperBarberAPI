using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Entities
{
    public class BarberShopBarber
    {
        public int BarberShopId { get; set; }

        [ForeignKey(nameof(BarberShopId))]
        public BarberShop BarberShop { get; set; }

        public int BarberId { get; set; }

        [ForeignKey(nameof(BarberId))]
        public Barber Barber { get; set; }

        public bool IsOwner { get; set; }

        public bool IsAvailable { get; set; }
    }
}
