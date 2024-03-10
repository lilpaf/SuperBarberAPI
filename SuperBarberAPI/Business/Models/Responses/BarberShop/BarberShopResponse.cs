namespace Business.Models.Responses.BarberShop
{
    public class BarberShopResponse
    {
        public required int Id { get; init; }

        public required string Name { get; init; }

        public required string Address { get; init; }
        
        public required string? About { get; init; }

        public required Dictionary<string, Tuple<string?, string?>> WorkingWeekHours { get; init; }

        //public required ICollection<BarberShopService> Services { get; init; }
        //public required ICollection<BarberShopBarber> Barbers { get; init; }

        public required double AverageRating { get; init; }
    }
}
