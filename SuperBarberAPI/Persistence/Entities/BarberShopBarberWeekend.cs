using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Entities
{
    public class BarberShopBarberWeekend
    {
        public int Id { get; set; }

        public required int BarberShopBarberId { get; set; }

        [ForeignKey(nameof(BarberShopBarberId))]
        public BarberShopBarber BarberShopBarber { get; set; } = null!;

        public required DateTime Start { get; set; }

        public required DateTime End { get; set; }
    }
}
