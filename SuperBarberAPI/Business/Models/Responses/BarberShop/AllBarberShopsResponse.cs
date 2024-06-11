using Business.Models.Dtos;

namespace Business.Models.Responses.BarberShop
{
    public class AllBarberShopsResponse
    {
        public required string City { get; init; }

        public string? Neighborhood { get; init; }

        public string? BarberShopSearchName { get; init; }

        public required IReadOnlyList<AllBarberShopDto> BarberShops { get; init; }
    }
}
