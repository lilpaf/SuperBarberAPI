using Persistence.Contexts;
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

        public async Task SaveUserAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
