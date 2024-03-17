using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;
using Persistence.Entities;
using Persistence.Interfaces;
using Persistence.Models;

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
        
        public async Task<IReadOnlyList<Barber>> GetAllActiveBarbersWithUsersAndBarberShopsAsync(QueryParameterContainer queryParams)
        {
            _logger.LogInformation("Getting active barbers from SQL Db");

            IQueryable<Barber> barbers = QueryBarbers(queryParams);

            return await barbers
                .Include(b => b.User)
                .Include(b => b.BarberShops)
                .ThenInclude(bs => bs.BarberShop)
                .ToListAsync();
        }
        
        public async Task AddBarberAsync(Barber barber)
        {
            _logger.LogInformation("Adding barber to SQL Db");

            await _context.Barbers.AddAsync(barber);
        }
        
        public void UpdateBarber(Barber barber)
        {
            _logger.LogInformation("Updating barber from SQL Db");

            _context.Barbers.Update(barber);
        }

        public async Task SaveChangesAsync()
        {
            _logger.LogInformation("Saving barber to SQL Db");
            await _context.SaveChangesAsync();
        }

        private IQueryable<Barber> QueryBarbers(QueryParameterContainer queryParams)
        {
            _logger.LogInformation("Querying barbers");

            IQueryable<Barber> barbersQuery = _context.Barbers
                .Where(b => !b.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrEmpty(queryParams.City))
            {
                barbersQuery.Where(b => b.BarberShops.Any(bs => bs.BarberShop.City.Name == queryParams.City));
            }
            
            if (!string.IsNullOrEmpty(queryParams.Neighborhood))
            {
                barbersQuery.Where(b => b.BarberShops.Any(bs => bs.BarberShop.Neighborhood != null 
                && bs.BarberShop.Neighborhood.Name == queryParams.Neighborhood));
            }

            if (!string.IsNullOrEmpty(queryParams.SearchName))
            {
                string searchName = queryParams.SearchName.Replace(" ", string.Empty);

                // Names will not be null since they will be null only when the user is deleted
                barbersQuery.Where(
                    b => b.User.FirstName!.Replace(" ", string.Empty)
                        .Contains(searchName, StringComparison.InvariantCultureIgnoreCase) || 
                    b.User.LastName!.Replace(" ", string.Empty)
                        .Contains(searchName, StringComparison.InvariantCultureIgnoreCase));
            }

            barbersQuery.Skip(queryParams.SkipCount);

            return barbersQuery;
        }
    }
}
