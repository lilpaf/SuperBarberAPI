using Business.Models.Requests.BarberShop;
using Business.Models.Responses.BarberShop;

namespace Business.Interfaces
{
    public interface IBarberShopService
    {
        Task<AllBarberShopsResponse> GetAllBarberShopsAsync(AllBarberShopRequest request);

        Task<RegisterBarberShopResponse> RegisterBarberShopAsync(RegisterBarberShopRequest request);
    }
}
