using System.ComponentModel.DataAnnotations;

namespace Persistence.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public ICollection<ServiceCategory> Services { get; set; } = new HashSet<ServiceCategory>();
    }
}
