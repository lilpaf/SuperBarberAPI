using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;
using Persistence.Entities;
using Persistence.Interfaces;

namespace Persistence.Implementations
{
    public class CityRepository : ICityRepository
    {
        private readonly SuperBarberDbContext _context;
        private readonly ILogger<CityRepository> _logger;

        public CityRepository(
            SuperBarberDbContext context,
            ILogger<CityRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IReadOnlyList<City>> GetAllCitiesAsync()
        {
            _logger.LogInformation("Getting all cities");

            return await _context.Cities
                .ToListAsync();
        }

        public async Task<City?> GetCityByNameAsync(string name)
        {
            _logger.LogInformation("Getting district by name");

            return await _context.Cities.FirstOrDefaultAsync(c => c.Name == name);
        }
    }
}
