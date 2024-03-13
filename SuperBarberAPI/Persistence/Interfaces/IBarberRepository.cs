using Persistence.Entities;

namespace Persistence.Interfaces
{
    public interface IBarberRepository
    {
        Task<bool> BarberExistsByUserIdAsync(string userId);

        Task<Barber?> GetActiveOrDeletedBarberByUserIdAsync(string userId);

        Task AddBarberAsync(Barber barber);

        Task SaveChangesAsync();
    }
}
