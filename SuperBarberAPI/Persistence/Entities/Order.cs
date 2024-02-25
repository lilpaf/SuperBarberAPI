using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public int BarberShopId { get; set; }

        [ForeignKey(nameof(BarberShopId))]
        public BarberShop BarberShop { get; set; }

        public DateTime Date { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeleteDate { get; set; }

        public decimal TotalPrice { get; set; }

        public ICollection<BarberShopService> Services { get; set; } = new HashSet<BarberShopService>();
        public ICollection<BarberOrder> Barbers { get; set; } = new HashSet<BarberOrder>();
    }
}
