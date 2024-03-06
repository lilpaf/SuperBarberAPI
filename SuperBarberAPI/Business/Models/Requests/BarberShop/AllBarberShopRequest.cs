namespace Business.Models.Requests.BarberShop
{
    //ToDo fix it when we create the FE
    public class AllBarberShopRequest
    {
        public required string City { get; init; }

        public string? Neighborhood { get; init; }

        public string? BarberShopSearchName { get; init; }

        public int CurrentPage { get; init; }
    }
}
