using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Entities
{
    public class District
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public int CityId { get; set; }
        
        [ForeignKey(nameof(CityId))]
        public City City { get; set; }
    }
}
