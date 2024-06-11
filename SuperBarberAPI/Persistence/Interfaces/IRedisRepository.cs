namespace Persistence.Interfaces
{
    public interface IRedisRepository
    {
        Task<string?> GetDataAsync(string key);

        Task SaveDataAsync(string key, string value, TimeSpan? expiry = null);
        
        void SaveData(string key, string value, TimeSpan? expiry = null);
    }
}
