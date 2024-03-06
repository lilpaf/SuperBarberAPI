using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;
using Persistence.Entities;
using Persistence.Interfaces;

namespace Persistence.Implementations
{
    public class CityRepository : ICityRepository
    {
        public const string CitiesKeyRedis = "Cities";
        private readonly IDatabase _redisDb;
        private readonly SuperBarberDbContext _context;
        private readonly ILogger<CityRepository> _logger;

        public CityRepository(
            SuperBarberDbContext context,
            ILogger<CityRepository> logger,
            IDatabase redisDb)
        {
            _context = context;
            _logger = logger;
            _redisDb = redisDb;
        }

        public async Task<IReadOnlyList<string>> GetAllCitiesNameFromRedisAsync()
        {
            _logger.LogInformation("Getting all cities from Redis Db");

            RedisValue[] citiesName = await _redisDb.ListRangeAsync(CitiesKeyRedis);

            return [.. citiesName];            
        }

        public async Task<IReadOnlyList<City>> GetAllCitiesAsync()
        {
            _logger.LogInformation("Getting all cities from SQL Db");

            return await _context.Cities.ToListAsync();
        }

        public async Task<City?> GetCityByNameAsync(string name)
        {
            _logger.LogInformation("Getting city by name from SQL Db");

            return await _context.Cities.FirstOrDefaultAsync(c => c.Name == name);
        }
    }
}
