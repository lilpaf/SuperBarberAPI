namespace Persistence.Dtos
{
    public class BarberShopDto
    {
        public required int Id { get; init; }

        public required string Name { get; init; }

        public required string City { get; init; }

        public required string District { get; init; }

        public required string Street { get; init; }

        public required string StartHour { get; init; }

        public required string FinishHour { get; init; }

        public required string? ImageName { get; init; }
    }
}
