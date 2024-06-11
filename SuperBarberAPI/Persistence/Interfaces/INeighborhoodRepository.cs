using Persistence.Entities;

namespace Persistence.Interfaces
{
    public interface INeighborhoodRepository
    {
        Task<IReadOnlyList<Neighborhood>> GetAllNeighborhoodsByCityIdAsync(int cityId);

        Task<Neighborhood?> GetNeighborhoodByNameAsync(string name);
    }
}
