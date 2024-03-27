namespace Business.Models.Dtos
{
    public class AllBarberShopDto
    {
        public required int Id { get; init; }

        public required string Name { get; init; }

        public required string Address { get; init; }

        public required Dictionary<string, DayHoursDto> WorkingWeekHoursToday { get; init; }

        public required double AverageRating { get; init; }

        //ToDo fix it
        //public required string ImageName { get; init; }
        
        //ToDo maybe add top 3 services to be displayed
    }
}
