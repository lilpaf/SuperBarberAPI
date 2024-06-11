namespace Business.Models.Dtos
{
    public class CityDto
    {
        public required int Id { get; set; }

        public required string Name { get; set; }

        public required IReadOnlyList<NeighborhoodDto>? Neighborhoods { get; set; }
    }
}
