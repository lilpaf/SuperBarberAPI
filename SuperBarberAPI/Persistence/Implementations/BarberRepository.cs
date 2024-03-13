using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;
using Persistence.Entities;
using Persistence.Interfaces;

namespace Persistence.Implementations
{
    public class BarberRepository : IBarberRepository
    {
        private readonly SuperBarberDbContext _context;
        private readonly ILogger<BarberRepository> _logger;

        public BarberRepository(
            SuperBarberDbContext context,
            ILogger<BarberRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> BarberExistsByUserIdAsync(string userId)
        {
            _logger.LogInformation("Checking if barber exists by user id {UserId} from SQL Db", userId);

            return await _context.Barbers.AnyAsync(b => b.UserId == userId);
        }
        
        public async Task<Barber?> GetActiveOrDeletedBarberByUserIdAsync(string userId)
        {
            _logger.LogInformation("Getting active or deleted barber by user id {UserId} from SQL Db", userId);

            return await _context.Barbers.FirstOrDefaultAsync(b => b.UserId == userId);
        }
        
        public async Task AddBarberAsync(Barber barber)
        {
            _logger.LogInformation("Adding barber to SQL Db");

            await _context.Barbers.AddAsync(barber);
        }

        public async Task SaveChangesAsync()
        {
            _logger.LogInformation("Saving barber to SQL Db");
            await _context.SaveChangesAsync();
        }
    }
}
