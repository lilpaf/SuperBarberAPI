using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;
using Persistence.Entities;
using Persistence.Interfaces;
using Persistence.Models;

namespace Persistence.Implementations
{
    public class BarberShopRepository : IBarberShopRepository
    {
        private readonly SuperBarberDbContext _context;
        private readonly ILogger<BarberShopRepository> _logger;

        public BarberShopRepository(
            SuperBarberDbContext context,
            ILogger<BarberShopRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IReadOnlyList<BarberShop>> GetAllPublicBarberShopsWithCitiesNeighborhoodsAndWorkingDaysAsync(QueryParameterContainer queryParams)
        {
            _logger.LogInformation("Getting all active barber shops from SQL Db");

            IQueryable<BarberShop> barberShopQuery = QueryBarberShops(queryParams);

            return await barberShopQuery
                .Include(b => b.City)
                .Include(b => b.Neighborhood)
                .Include(b => b.BarberShopWorkingDays)
                .ThenInclude(d => d.WeekDay)
                .ToListAsync();
        }

        public async Task<int> GetTotalNumberActiveBarberShopsAsync()
        {
            _logger.LogInformation("Getting total number of all active barber shops from SQL Db");

            return await _context.BarberShops.CountAsync();
        }

        public async Task<BarberShop?> GetPublicAndPrivateBarberShopWithCitiesNeighborhoodsAndWorkingDaysByIdAsync(int id)
        {
            _logger.LogInformation("Getting public and private barber shop by id from SQL Db");

            return await _context.BarberShops
                .Include(b => b.City)
                .Include(b => b.Neighborhood)
                .Include(b => b.BarberShopWorkingDays)
                .ThenInclude(d => d.WeekDay)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        }

        public async Task<BarberShop?> GetOnlyPublicBarberShopWithCitiesNeighborhoodsAndWorkingDaysByIdAsync(int id)
        {
            _logger.LogInformation("Getting only public barber shop by id from SQL Db");

            return await _context.BarberShops
                .Include(b => b.City)
                .Include(b => b.Neighborhood)
                .Include(b => b.BarberShopWorkingDays)
                .ThenInclude(d => d.WeekDay)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted && b.IsPublic);
        }

        public async Task AddBarberShopAsync(BarberShop barberShop)
        {
            _logger.LogInformation("Adding barber shop to SQL Db");
            await _context.BarberShops.AddAsync(barberShop);
        }

        public void UpdateBarberShopAsync(BarberShop barberShop)
        {
            _logger.LogInformation("Updating barber shop from SQL Db");
            _context.BarberShops.Update(barberShop);
        }

        public async Task SaveChangesAsync()
        {
            _logger.LogInformation("Saving barber shop to SQL Db");
            await _context.SaveChangesAsync();
        }

        private IQueryable<BarberShop> QueryBarberShops(QueryParameterContainer queryParams)
        {
            _logger.LogInformation("Querying barber shops");

            IQueryable<BarberShop> barberShopQuery = _context.BarberShops
                .Where(b => b.IsPublic && !b.IsDeleted && b.City.Name == queryParams.City)
                .AsQueryable();

            if (!string.IsNullOrEmpty(queryParams.Neighborhood))
            {
                barberShopQuery.Where(b => b.Neighborhood != null && b.Neighborhood.Name == queryParams.Neighborhood);
            }

            if (!string.IsNullOrEmpty(queryParams.SearchName))
            {
                string searchName = queryParams.SearchName.Replace(" ", string.Empty);

                barberShopQuery.Where(b => b.Name.Replace(" ", string.Empty)
                .Contains(searchName, StringComparison.InvariantCultureIgnoreCase));
            }

            barberShopQuery.Skip(queryParams.SkipCount);

            return barberShopQuery;
        }
    }
}
