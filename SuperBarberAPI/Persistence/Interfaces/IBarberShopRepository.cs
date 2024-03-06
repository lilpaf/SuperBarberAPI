using Persistence.Dtos;
using Persistence.Entities;
using Persistence.Models;

namespace Persistence.Interfaces
{
    public interface IBarberShopRepository
    {
        Task<IReadOnlyList<BarberShopDto>> GetAllActiveBarberShopsAsync(QueryParameterContainer queryParams);

        Task<int> GetTotalNumberActiveBarberShopsAsync();

        Task AddBarberShopAsync(BarberShop barberShop);

        Task SaveChangesAsync();
    }
}
