using Business.Models.Requests.BarberShop;
using Business.Models.Responses.BarberShop;
using Microsoft.AspNetCore.JsonPatch;

namespace Business.Interfaces
{
    public interface IBarberShopService
    {
        Task<AllBarberShopsResponse> GetAllBarberShopsAsync(AllBarberShopRequest request);

        Task<RegisterBarberShopResponse> RegisterBarberShopAsync(RegisterBarberShopRequest request);

        Task<UpdateBarberShopResponse> UpdateBarberShopAsync(int barberShopId, JsonPatchDocument<UpdateBarberShopRequest> patchDoc);
    }
}
