using Persistence.Dtos;
using Persistence.Entities;

namespace Persistence.Interfaces
{
    public interface IBarberShopRepository
    {
        Task<IReadOnlyList<BarberShopDto>> GetAllActiveBarberShopsAsync();
    }
}
