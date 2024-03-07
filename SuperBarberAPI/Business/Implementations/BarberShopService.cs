using Business.Constants.Messages;
using Business.Interfaces;
using Business.Models.Exceptions;
using Business.Models.Requests.BarberShop;
using Business.Models.Responses.BarberShop;
using Microsoft.Extensions.Logging;
using Persistence.Dtos;
using Persistence.Entities;
using Persistence.Interfaces;
using Persistence.Models;

namespace Business.Implementations
{
    public class BarberShopService : IBarberShopService
    {
        private readonly IBarberShopRepository _barberShopRepository;
        private readonly ICityRepository _cityRepository;
        private readonly INeighborhoodRepository _neighborhoodRepository;
        private readonly ILogger<UserService> _logger;

        public BarberShopService(
            ILogger<UserService> logger,
            IBarberShopRepository barberShopRepository,
            ICityRepository cityRepository,
            INeighborhoodRepository neighborhoodRepository)
        {
            _logger = logger;
            _barberShopRepository = barberShopRepository;
            _cityRepository = cityRepository;
            _neighborhoodRepository = neighborhoodRepository;
        }

        public async Task<AllBarberShopsResponse> GetAllBarberShopsAsync(AllBarberShopRequest request)
        {
            QueryParameterContainer queryParams = new()
            { 
                City = request.City,
                Neighborhood = request.Neighborhood,
                BarberShopSearchName = request.BarberShopSearchName
            };

            //ToDo may be needed if not delete the method
            //int totalActiveBarberShops = await _barberShopRepository.GetTotalNumberActiveBarberShopsAsync();
            IReadOnlyList<string> citiesName = await _cityRepository.GetAllCitiesNameFromRedisAsync();

            if (!citiesName.Any())
            {
                IReadOnlyList<City> cities = await _cityRepository.GetAllCitiesAsync();

                citiesName = cities.Select(c => c.Name).ToList();
            }

            City? city = await _cityRepository.GetCityByNameAsync(request.City);

            if (city is null) 
            {
                _logger.LogError("{City} city dose not exists", request.City);
                throw new InvalidArgumentException(Messages.InvalidCity);
            }

            IReadOnlyList<string> neighborhoodsName = await _neighborhoodRepository.GetAllNeighborhoodsNameByCityNameFromRedisAsync(city.Name);

            if (!neighborhoodsName.Any())
            {
                IReadOnlyList<Neighborhood> neighborhoods = await _neighborhoodRepository.GetAllNeighborhoodsByCityIdAsync(city.Id);
                
                neighborhoodsName = neighborhoods.Select(n => n.Name).ToList();
            }

            IReadOnlyList<BarberShopDto> activeBarberShops = await _barberShopRepository.GetAllActiveBarberShopsAsync(queryParams);

            return new AllBarberShopsResponse()
            {
                City = request.City,
                Cities = citiesName,
                Neighborhood = request.Neighborhood,
                Neighborhoods = neighborhoodsName,
                BarberShopSearchName = request.BarberShopSearchName,
                //TotalPages = totalActiveBarberShops / QueryParameterContainer.BarberShopsPerPage, //ToDo may be needed
                BarberShops = activeBarberShops
            };
        }
        
        public async Task<RegisterBarberShopResponse> RegisterBarberShopAsync(RegisterBarberShopRequest request)
        {
            TimeSpan startHour;
            TimeSpan finishHour;

            City? city = await _cityRepository.GetCityByNameAsync(request.City);

            if (city is null)
            {
                _logger.LogError("{City} city dose not exists", request.City);
                throw new InvalidArgumentException(Messages.InvalidCity);
            }

            Neighborhood? neighborhood = null;

            if (!string.IsNullOrEmpty(request.Neighborhood))
            {
                neighborhood = await _neighborhoodRepository.GetNeighborhoodByNameAsync(request.Neighborhood);

                if (neighborhood is null)
                {
                    _logger.LogError("{Neighborhood} neighborhood dose not exists", request.Neighborhood);
                    throw new InvalidArgumentException(Messages.InvalidNeighborhood);
                }
            }

            (startHour, finishHour) = ParseHours(request.StartHour, request.FinishHour);

            BarberShop barberShop = new()
            {
                Name = request.Name,
                CityId = city.Id,
                Neighborhood = neighborhood,
                Address = request.Address,
                StartHour = startHour,
                FinishHour = finishHour,
                IsPublic = false,
                IsDeleted = false,
                AverageRating = 0,
            };

            await _barberShopRepository.AddBarberShopAsync(barberShop);

            await _barberShopRepository.SaveChangesAsync();

            return new RegisterBarberShopResponse()
            {
                Message = Messages.BarberShopRegistrationSussesfuly
            };
        }

        private static Tuple<TimeSpan, TimeSpan> ParseHours(string startHour, string finishHour)
        {
            string[] startHourArr = startHour.Split(':');
            string[] finishHourArr = finishHour.Split(':');

            TimeSpan startHourParsed = new (int.Parse(startHourArr[0]), int.Parse(startHourArr[1]), 0);

            TimeSpan finishHourParsed = new (int.Parse(finishHourArr[0]), int.Parse(finishHourArr[1]), 0);

            return Tuple.Create(startHourParsed, finishHourParsed);
        }
    }
}
