using Persistence.Entities;

namespace Persistence.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> FindUserByEmailAsync(string email);
    }
}
