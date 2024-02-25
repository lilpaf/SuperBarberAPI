using Persistence.Contexts;
using Persistence.Entities;
using Persistence.Interfaces;

namespace Persistence.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly SuperBarberDbContext _context;

        public UserRepository(SuperBarberDbContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveUserAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
