using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Entities
{
    public class Neighborhood
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public required int CityId { get; set; }
        
        [ForeignKey(nameof(CityId))]
        public City City { get; set; } = null!;
    }
}
