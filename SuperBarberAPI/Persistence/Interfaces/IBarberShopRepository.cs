using Persistence.Entities;
using Persistence.Models;

namespace Persistence.Interfaces
{
    public interface IBarberShopRepository
    {
        Task<IReadOnlyList<BarberShop>> GetAllActiveBarberShopsWithCitiesAndNeighborhoodsAsync(QueryParameterContainer queryParams);

        Task<int> GetTotalNumberActiveBarberShopsAsync();

        Task<BarberShop?> GetBarberShopWithCityAndNeighborhoodByIdAsync(int id);

        Task AddBarberShopAsync(BarberShop barberShop);

        void UpdateBarberShopAsync(BarberShop barberShop);

        Task SaveChangesAsync();
    }
}
