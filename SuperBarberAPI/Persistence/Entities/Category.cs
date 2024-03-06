using System.ComponentModel.DataAnnotations;

namespace Persistence.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public ICollection<ServiceCategory> Services { get; set; } = new HashSet<ServiceCategory>();
    }
}
