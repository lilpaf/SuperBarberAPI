using Persistence.Dtos;

namespace Persistence.Interfaces
{
    public interface IBarberShopRepository
    {
        Task<IReadOnlyList<BarberShopDto>> GetAllActiveBarberShopsAsync();
    }
}
