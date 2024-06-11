using Persistence.Entities;

namespace Persistence.Interfaces
{
    public interface ICityRepository
    {
        Task<IReadOnlyList<City>> GetAllCitiesAsync();

        Task<City?> GetCityByNameAsync(string name);
    }
}
