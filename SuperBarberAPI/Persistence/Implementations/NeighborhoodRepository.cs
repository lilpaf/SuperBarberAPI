using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;
using Persistence.Entities;
using Persistence.Interfaces;

namespace Persistence.Implementations
{
    public class NeighborhoodRepository : INeighborhoodRepository
    {
        private readonly SuperBarberDbContext _context;
        private readonly ILogger<NeighborhoodRepository> _logger;

        public NeighborhoodRepository(
            SuperBarberDbContext context,
            ILogger<NeighborhoodRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IReadOnlyList<Neighborhood>> GetAllNeighborhoodsByCityIdAsync(int cityId)
        {
            _logger.LogInformation("Getting all neighborhoods by city");

            return await _context.Neighborhoods
                .Where(x => x.CityId == cityId)
                .ToListAsync();
        }
        
        public async Task<Neighborhood?> GetNeighborhoodByNameAsync(string name)
        {
            _logger.LogInformation("Getting neighborhood by name");

            return await _context.Neighborhoods.FirstOrDefaultAsync(d => d.Name == name);
        }
    }
}
