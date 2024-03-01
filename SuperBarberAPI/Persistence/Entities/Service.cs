using System.ComponentModel.DataAnnotations;

namespace Persistence.Entities
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public bool IsDeleted { get; set; }

        public DateTime? DeleteDate { get; set; }

        public ICollection<ServiceCategory> Categories { get; set; } = new HashSet<ServiceCategory>();
    }
}
