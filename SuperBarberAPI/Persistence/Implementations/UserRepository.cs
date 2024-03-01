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
            _logger.LogInformation("Checking for user with {email}", email);

            return await _context.Users.AnyAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task AddUserRefreshTokenAsync(UserRefreshToken refreshToken)
        {
            _logger.LogInformation("Adding user refresh tokens");
            await _context.UserRefreshTokens.AddAsync(refreshToken);
        }

        public async Task SaveChangesAsync()
        {
            _logger.LogInformation("Saving user information");
            await _context.SaveChangesAsync();
        }
    }
}
