using Persistence.Entities;

namespace Persistence.Interfaces
{
    public interface IUserRepository
    {
        Task SaveUserAsync();

        Task AddUserAsync(User user);
    }
}
