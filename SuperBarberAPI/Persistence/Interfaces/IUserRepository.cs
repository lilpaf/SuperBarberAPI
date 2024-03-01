using Persistence.Entities;

namespace Persistence.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> UserIsActiveAndExistsByEmailAsync(string email);

        Task AddUserRefreshTokenAsync(UserRefreshToken refreshToken);

        Task SaveChangesAsync();
    }
}
