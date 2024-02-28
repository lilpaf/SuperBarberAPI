using Business.Models.Responses;

namespace Business.Interfaces
{
    public interface IBarberShopService
    {
        Task<AllBarberShopsResponse> GetAllBarberShopsAsync();
    }
}
