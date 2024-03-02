using Persistence.Entities;

namespace Persistence.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> UserIsActiveAndExistsByEmailAsync(string email);

        Task<UserRefreshToken?> GetUserRefreshTokenWithUserByTokenAsync(string token);

        Task<UserRefreshToken?> GetUserRefreshTokenByUserIdAsync(string userId);

        Task AddUserRefreshTokenAsync(UserRefreshToken refreshToken);

        void UpdateUserRefreshToken(UserRefreshToken refreshToken);

        Task SaveChangesAsync();
    }
}
