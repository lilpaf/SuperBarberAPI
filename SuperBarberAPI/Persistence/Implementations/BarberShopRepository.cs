using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;
using Persistence.Dtos;
using Persistence.Entities;
using Persistence.Interfaces;
using Persistence.Models;

namespace Persistence.Implementations
{
    public class BarberShopRepository : IBarberShopRepository
    {
        private const string StringHourFormat = @"hh\:mm";
        private readonly SuperBarberDbContext _context;
        private readonly ILogger<BarberShopRepository> _logger;

        public BarberShopRepository(
            SuperBarberDbContext context,
            ILogger<BarberShopRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IReadOnlyList<BarberShopDto>> GetAllActiveBarberShopsAsync(QueryParameterContainer queryParams)
        {
            _logger.LogInformation("Getting all active barbershops");

            IQueryable<BarberShop> barberShopQuery = QueryBarberShops(queryParams);

            return await barberShopQuery
                .Select(b => new BarberShopDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Address = b.Address,
                    StartHour = b.StartHour.ToString(StringHourFormat),
                    FinishHour = b.FinishHour.ToString(StringHourFormat),
                    //ToDo fix it
                    //ImageName = b.ImageName
                })
                .ToListAsync();
        }
        
        public async Task<int> GetTotalNumberActiveBarberShopsAsync()
        {
            _logger.LogInformation("Getting total number of all active barbershops");

            return await _context.BarberShops.CountAsync();
        }

        public async Task AddBarberShopAsync(BarberShop barberShop)
        {
            _logger.LogInformation("Adding barbershop");
            await _context.BarberShops.AddAsync(barberShop);
        }

        public async Task SaveChangesAsync()
        {
            _logger.LogInformation("Saving barbershop");
            await _context.SaveChangesAsync();
        }

        private IQueryable<BarberShop> QueryBarberShops(QueryParameterContainer queryParams) 
        {
            _logger.LogInformation("Querying barbershops");

            IQueryable<BarberShop> barberShopQuery = _context.BarberShops
                .Where(b => b.IsPublic && !b.IsDeleted && b.City.Name == queryParams.City)
                .AsQueryable();

            if (!string.IsNullOrEmpty(queryParams.Neighborhood))
            {
                barberShopQuery.Where(b => b.Neighborhood != null && b.Neighborhood.Name == queryParams.Neighborhood);
            }
            
            if (!string.IsNullOrEmpty(queryParams.BarberShopSearchName))
            {
                barberShopQuery.Where(b => b.Name.ToLower().Replace(" ", string.Empty)
                .Contains(queryParams.BarberShopSearchName.ToLower().Replace(" ", string.Empty)));
            }

            barberShopQuery.Skip(queryParams.SkipCount);

            return barberShopQuery;
        }
    }
}
