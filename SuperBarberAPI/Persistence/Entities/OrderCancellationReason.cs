using Persistence.Enums;
using System.ComponentModel.DataAnnotations;

namespace Persistence.Entities
{
    public class OrderCancellationReason
    {
        public int Id { get; set; }

        [Required]
        public required string Reason { get; set; }

        [Required]
        [EnumDataType(typeof(CancellationReasonEnum))]
        public required CancellationReasonEnum CancellationReasonEnum { get; set; }

        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}
