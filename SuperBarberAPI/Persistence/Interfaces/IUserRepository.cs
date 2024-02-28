using Persistence.Entities;

namespace Persistence.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> UserExistsByEmailAsync(string email);
    }
}
