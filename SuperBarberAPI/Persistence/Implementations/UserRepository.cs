using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;
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

        public async Task<bool> FindUserByEmailAsync(string email)
        {
            _logger.LogInformation("Getting user with {email}", email);

            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}
