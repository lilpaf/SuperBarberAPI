using System.ComponentModel.DataAnnotations;

namespace Persistence.Entities
{
    public class WeekDay
    {
        public int Id { get; set; }

        [Required]
        public required string DayOfWeekName { get; set; }

        [Required]
        [EnumDataType(typeof(DayOfWeek))]
        public DayOfWeek DayOfWeekEnum { get; set; }

        public ICollection<BarberShopWorkingDay> BarberShopWorkingDays { get; set; } = new HashSet<BarberShopWorkingDay>();
    }
}
