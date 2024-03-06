namespace Persistence.Dtos
{
    public class BarberShopDto
    {
        public required int Id { get; init; }

        public required string Name { get; init; }

        public required string Address { get; init; }

        public required string StartHour { get; init; }

        public required string FinishHour { get; init; }

        //ToDo fix it
        //public required string ImageName { get; init; }
    }
}
