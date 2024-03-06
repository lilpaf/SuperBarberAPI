using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Entities
{
    public class ServiceCategory
    {
        public required int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        public required int ServiceId { get; set; }

        [ForeignKey(nameof(ServiceId))]
        public Service Service { get; set; } = null!;
    }
}
