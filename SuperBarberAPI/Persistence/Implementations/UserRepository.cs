using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;
using Persistence.Entities;
using Persistence.Interfaces;

namespace Persistence.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly SuperBarberDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(
            SuperBarberDbContext context, 
            ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> UserIsActiveAndExistsByEmailAsync(string email)
        {
            _logger.LogInformation("Checking for user with {Email}", email);

            return await _context.Users.AnyAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<UserRefreshToken?> GetUserRefreshTokenWithUserByTokenAsync(string token)
        {
            _logger.LogInformation("Getting refresh token for token {Token}", token);
            return await _context.UserRefreshTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token);
        }
        
        public async Task<UserRefreshToken?> GetUserRefreshTokenByUserIdAsync(string userId)
        {
            _logger.LogInformation("Getting refresh token for user {Token}", userId);
            return await _context.UserRefreshTokens.FirstOrDefaultAsync(t => t.UserId == userId);
        }

        public async Task AddUserRefreshTokenAsync(UserRefreshToken refreshToken)
        {
            _logger.LogInformation("Adding refresh token {Id} to user {UserId}", refreshToken.Id, refreshToken.UserId);
            await _context.UserRefreshTokens.AddAsync(refreshToken);
        }

        public void UpdateUserRefreshToken(UserRefreshToken refreshToken)
        {
            _logger.LogInformation("Updating refresh token {Id}", refreshToken.Id);
            _context.UserRefreshTokens.Update(refreshToken);
        }

        public async Task SaveChangesAsync()
        {
            _logger.LogInformation("Saving user information");
            await _context.SaveChangesAsync();
        }
    }
}
