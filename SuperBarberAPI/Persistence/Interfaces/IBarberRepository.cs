using Persistence.Entities;
using Persistence.Models;

namespace Persistence.Interfaces
{
    public interface IBarberRepository
    {
        Task<bool> BarberExistsByUserIdAsync(string userId);

        Task<Barber?> GetActiveOrDeletedBarberByUserIdAsync(string userId);

        Task<IReadOnlyList<Barber>> GetAllActiveBarbersWithUsersAndBarberShopsAsync(QueryParameterContainer queryParams);

        Task AddBarberAsync(Barber barber);

        void UpdateBarber(Barber barber);

        Task SaveChangesAsync();
    }
}
