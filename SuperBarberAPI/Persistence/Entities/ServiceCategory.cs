using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Entities
{
    public class ServiceCategory
    {
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        public int ServiceId { get; set; }

        [ForeignKey(nameof(ServiceId))]
        public Service Service { get; set; }
    }
}
