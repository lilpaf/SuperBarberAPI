using Persistence.Entities;
using Persistence.Models;

namespace Persistence.Interfaces
{
    public interface IBarberShopRepository
    {
        Task<IReadOnlyList<BarberShop>> GetAllPublicBarberShopsWithCitiesNeighborhoodsAndWorkingDaysAsync(QueryParameterContainer queryParams);

        Task<int> GetTotalNumberActiveBarberShopsAsync();

        Task<BarberShop?> GetPublicAndPrivateBarberShopWithCitiesNeighborhoodsAndWorkingDaysByIdAsync(int id);

        Task<BarberShop?> GetOnlyPublicBarberShopWithCitiesNeighborhoodsAndWorkingDaysByIdAsync(int id);

        Task AddBarberShopAsync(BarberShop barberShop);

        void UpdateBarberShopAsync(BarberShop barberShop);

        Task SaveChangesAsync();
    }
}