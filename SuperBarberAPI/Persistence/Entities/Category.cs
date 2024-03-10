using Persistence.Enums;
using System.ComponentModel.DataAnnotations;

namespace Persistence.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public required string CategoryName { get; set; }
        
        [Required]
        [EnumDataType(typeof(CategoryEnum))]
        public required CategoryEnum CategoryEnum { get; set; }

        public ICollection<ServiceCategory> Services { get; set; } = new HashSet<ServiceCategory>();
    }
}
