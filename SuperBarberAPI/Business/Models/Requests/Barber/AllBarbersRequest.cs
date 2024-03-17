namespace Business.Models.Requests.Barber
{
    public class AllBarbersRequest
    {
        public string? BarberShopCity { get; init; }

        public string? BarberShopNeighborhood { get; init; }

        public string? BarberName { get; init; }

        public int CurrentPage { get; init; }
    }
}
