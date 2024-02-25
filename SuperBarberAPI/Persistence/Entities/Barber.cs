using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Persistence.Entities
{
    public class Barber
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeleteDate { get; set; }

        public ICollection<BarberOrder> Orders { get; set; } = new HashSet<BarberOrder>();

        public ICollection<BarberShopBarber> BarberShops { get; set; } = new HashSet<BarberShopBarber>();
    }
}
