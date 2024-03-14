using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        [Required]
        public required string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        public required int BarberShopId { get; set; }

        [ForeignKey(nameof(BarberShopId))]
        public BarberShop BarberShop { get; set; } = null!;

        public required int BarberId { get; set; }

        [ForeignKey(nameof(BarberId))]
        public Barber Barber { get; set; }

        public required DateTime StartDate { get; set; }
        
        public required DateTime EndDate { get; set; }

        public required bool IsDeleted { get; set; }

        public required DateTime? DeleteDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public required decimal TotalPrice { get; set; }

        public ICollection<BarberShopService> Services { get; set; } = new HashSet<BarberShopService>();
    }
}
