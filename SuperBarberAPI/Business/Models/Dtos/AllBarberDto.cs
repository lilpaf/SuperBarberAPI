using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models.Dtos
{
    public class AllBarberDto
    {
        public required int Id { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required double AverageRating { get; init; }
        public required IReadOnlyList<string> BarberShopsNamesEmployed { get; init; }
    }
}
