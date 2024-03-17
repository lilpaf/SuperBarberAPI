using System.ComponentModel.DataAnnotations;

namespace Business.Models.Requests.BarberShop
{
    //ToDo fix it when we create the FE
    public class AllBarberShopRequest
    {
        [Required]
        public required string City { get; init; }

        public string? Neighborhood { get; init; }

        public string? BarberShopName { get; init; }

        public int CurrentPage { get; init; }
    }
}
