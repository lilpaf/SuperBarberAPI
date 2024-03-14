using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Entities
{
    public class BarberShopWorkingDay
    {
        public int Id { get; set; }

        public required int WeekDayId { get; set; }
        
        [ForeignKey(nameof(WeekDayId))]
        public WeekDay WeekDay { get; set; } = null!;

        public int BarberShopId { get; set; }

        [ForeignKey(nameof(BarberShopId))]
        public BarberShop BarberShop { get; set; } = null!;

        public required TimeSpan? OpeningHour { get; set; }

        public required TimeSpan? ClosingHour { get; set; }
    }
}
