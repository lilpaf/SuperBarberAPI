using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;
using Persistence.Entities;
using Persistence.Interfaces;
using StackExchange.Redis;

namespace Persistence.Implementations
{
    public class NeighborhoodRepository : INeighborhoodRepository
    {
        private readonly IDatabase _redisDb;
        private readonly SuperBarberDbContext _context;
        private readonly ILogger<NeighborhoodRepository> _logger;

        public NeighborhoodRepository(
            SuperBarberDbContext context,
            ILogger<NeighborhoodRepository> logger,
            IDatabase redisDb)
        {
            _context = context;
            _logger = logger;
            _redisDb = redisDb;
        }

        public async Task<IReadOnlyList<string>> GetAllNeighborhoodsNameByCityNameFromRedisAsync(string cityName)
        {
            _logger.LogInformation("Getting all neighborhoods by city name from Redis Db");

            RedisValue[] neighborhoodsName = await _redisDb.ListRangeAsync(cityName);

            return [.. neighborhoodsName];
        }
        
        public async Task<IReadOnlyList<Neighborhood>> GetAllNeighborhoodsByCityIdAsync(int cityId)
        {
            _logger.LogInformation("Getting all neighborhoods by city id from SQL Db");

            return await _context.Neighborhoods
                .Where(x => x.CityId == cityId)
                .ToListAsync();
        }
        
        public async Task<Neighborhood?> GetNeighborhoodByNameAsync(string name)
        {
            _logger.LogInformation("Getting neighborhood by name from SQL Db");

            return await _context.Neighborhoods.FirstOrDefaultAsync(d => d.Name == name);
        }
    }
}
