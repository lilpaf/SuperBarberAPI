using Persistence.Dtos;

namespace Business.Models.Responses.BarberShop
{
    //ToDo fix it when we create the FE
    public class AllBarberShopsResponse
    {
        public required string City { get; init; }

        public required IReadOnlyList<string> Cities { get; init; }

        public string? Neighborhood { get; init; }

        public required IReadOnlyList<string> Neighborhoods { get; init; }

        public string? BarberShopSearchName { get; init; }

        public required IReadOnlyList<BarberShopDto> BarberShops { get; init; }
    }
}
