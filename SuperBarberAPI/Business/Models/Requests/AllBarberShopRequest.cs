using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models.Requests
{
    //ToDo fix it when we create the FE
    public class AllBarberShopRequest
    {
        public const int BarberShopsPerPage = 6;

        public string City { get; init; }

        public IEnumerable<string> Cities { get; init; }

        public string District { get; init; }

        public IEnumerable<string> Districts { get; init; }

        public string SearchTerm { get; init; }

        public int CurrentPage { get; init; } = 1;

        public int TotalBarberShops { get; set; }

        //public BarberShopSorting Sorting { get; init; }

        //public IEnumerable<BarberShopListingViewModel> BarberShops { get; init; }
    }
}
