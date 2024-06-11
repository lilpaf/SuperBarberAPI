using Business.Models.Dtos;

namespace Business.Interfaces
{
    public interface ICityService
    {
        Task<IReadOnlyList<CityDto>> GetCitiesAndNeighborhoodsAsync();
    }
}
