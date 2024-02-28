using Persistence.Dtos;

namespace Business.Models.Responses
{
    //ToDo fix it when we create the FE
    public class AllBarberShopsResponse
    {
        //ToDo prob will me moved
        //public const int BarberShopsPerPage = 6;

        //public string City { get; init; }

        //public IEnumerable<string> Cities { get; init; }

        //public string District { get; init; }

        //public IEnumerable<string> Districts { get; init; }

        //public string SearchTerm { get; init; }

        //public int CurrentPage { get; init; } = 1;

        //public int TotalBarberShops { get; set; }

        //public BarberShopSorting Sorting { get; init; }

        public required IReadOnlyList<BarberShopDto> BarberShops { get; init; }
    }
}
