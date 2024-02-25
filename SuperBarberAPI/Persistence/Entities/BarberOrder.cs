using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Entities
{
    public class BarberOrder
    {
        public Guid OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }

        public int BarberId { get; set; }

        [ForeignKey(nameof(BarberId))]
        public Barber Barber { get; set; }
    }
}
