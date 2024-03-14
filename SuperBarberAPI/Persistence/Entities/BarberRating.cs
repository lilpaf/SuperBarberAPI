using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Entities
{
    public class BarberRating
    {
        public int Id { get; set; }

        public required int Star { get; set; }

        public required int BarberId { get; set; }

        [ForeignKey(nameof(BarberId))]
        public Barber Barber { get; set; } = null!;
    }
}
