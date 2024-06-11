using Microsoft.Extensions.Logging;
using Persistence.Interfaces;
using StackExchange.Redis;

namespace Persistence.Implementations
{
    public class RedisRepository : IRedisRepository
    {
        private readonly IDatabase _redisDb;
        private readonly ILogger<RedisRepository> _logger;

        public RedisRepository(IDatabase redisDb, ILogger<RedisRepository> logger)
        {
            _redisDb = redisDb;
            _logger = logger;
        }

        public async Task<string?> GetDataAsync(string key)
        {
            _logger.LogInformation("Getting data from Redis Db");

            string? data = await _redisDb.StringGetAsync(key);

            return data;
        }
        
        public async Task SaveDataAsync(string key, string value, TimeSpan? expiry = null)
        {
            _logger.LogInformation("Saving data to Redis Db");

            await _redisDb.StringSetAsync(key, value, expiry);
        }
        
        public void SaveData(string key, string value, TimeSpan? expiry = null)
        {
            _logger.LogInformation("Saving data to Redis Db");

            _redisDb.StringSet(key, value, expiry);
        }
    }
}
