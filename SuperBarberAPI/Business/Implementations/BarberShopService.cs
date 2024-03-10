using Business.Constants.Messages;
using Business.Interfaces;
using Business.Models.Dtos;
using Business.Models.Exceptions;
using Business.Models.Requests.BarberShop;
using Business.Models.Responses.BarberShop;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Persistence.Entities;
using Persistence.Interfaces;
using Persistence.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Business.Implementations
{
    public class BarberShopService : IBarberShopService
    {
        private const string StringHourFormat = @"hh\:mm";
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

            IReadOnlyList<BarberShop> activeBarberShops = await _barberShopRepository.GetAllActiveBarberShopsWithCitiesAndNeighborhoodsAsync(queryParams);

            IReadOnlyList<BarberShopDto> activeBarberShopsDto = activeBarberShops
                .Select(b => new BarberShopDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Address = b.Address,
                    StartHour = b.StartHour.ToString(StringHourFormat),
                    FinishHour = b.FinishHour.ToString(StringHourFormat),
                    AverageRating = b.AverageRating,
                    //ToDo fix it
                    //ImageName = b.ImageName
                })
                .ToList();

            return new AllBarberShopsResponse()
            {
                City = request.City,
                Cities = citiesName,
                Neighborhood = request.Neighborhood,
                Neighborhoods = neighborhoodsName,
                BarberShopSearchName = request.BarberShopSearchName,
                //TotalPages = totalActiveBarberShops / QueryParameterContainer.BarberShopsPerPage, //ToDo may be needed
                BarberShops = activeBarberShopsDto
            };
        }
        
        public async Task<RegisterBarberShopResponse> RegisterBarberShopAsync(RegisterBarberShopRequest request)
        {
            City? city;
            Neighborhood? neighborhood;
            
            (city, neighborhood) = await GetCityAndNeighborhoodByName(request.City, request.Neighborhood);

            TimeSpan startHour;
            TimeSpan finishHour;

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

        public async Task<UpdateBarberShopResponse> UpdateBarberShopAsync(int barberShopId, JsonPatchDocument<UpdateBarberShopRequest> patchDoc)
        {
            BarberShop? barberShopFromRepo = await _barberShopRepository.GetBarberShopWithCityAndNeighborhoodByIdAsync(barberShopId);

            if (barberShopFromRepo is null)
            {
                _logger.LogError("Barber shop with this id {Id} dose not exists", barberShopId);
                throw new InvalidArgumentException(Messages.BarberShopDoseNotExist);
            }

            UpdateBarberShopRequest updatedBarberShop = new()
            {
                Name = barberShopFromRepo.Name,
                City = barberShopFromRepo.City.Name,
                Neighborhood = barberShopFromRepo.Neighborhood?.Name,
                Address = barberShopFromRepo.Address,
                StartHour = barberShopFromRepo.StartHour.ToString(StringHourFormat),
                FinishHour = barberShopFromRepo.FinishHour.ToString(StringHourFormat)
            };

            patchDoc.ApplyTo(updatedBarberShop);

            List<ValidationResult> validationResults = new ();
            ValidationContext validationContext = new (updatedBarberShop, null, null);

            if (!Validator.TryValidateObject(updatedBarberShop, validationContext, validationResults, true))
            {
                string[] errorsMessages = validationResults
                    .Select(x => x.ErrorMessage ?? string.Empty)
                    .ToArray();

                throw new InvalidModelStateException(errorsMessages);
            }

            City? city;
            Neighborhood? neighborhood;

            (city, neighborhood) = await GetCityAndNeighborhoodByName(updatedBarberShop.City, updatedBarberShop.Neighborhood);

            TimeSpan startHour;
            TimeSpan finishHour;

            (startHour, finishHour) = ParseHours(updatedBarberShop.StartHour, updatedBarberShop.FinishHour);

            barberShopFromRepo.Name = updatedBarberShop.Name;
            barberShopFromRepo.City = city;
            barberShopFromRepo.Neighborhood = neighborhood;
            barberShopFromRepo.Address = updatedBarberShop.Address;
            barberShopFromRepo.StartHour = startHour;
            barberShopFromRepo.FinishHour = finishHour;

            _barberShopRepository.UpdateBarberShopAsync(barberShopFromRepo);

            await _barberShopRepository.SaveChangesAsync();

            return new UpdateBarberShopResponse()
            {
                Message = Messages.BarberShopUpdatedSussesfuly
            };
        }

        private async Task<(City, Neighborhood?)> GetCityAndNeighborhoodByName(string cityName, string? neighborhoodName)
        {
            City? city = await _cityRepository.GetCityByNameAsync(cityName);

            if (city is null)
            {
                _logger.LogError("{City} city dose not exists", cityName);
                throw new InvalidArgumentException(Messages.InvalidCity);
            }

            Neighborhood? neighborhood = null;

            if (!string.IsNullOrEmpty(neighborhoodName))
            {
                neighborhood = await _neighborhoodRepository.GetNeighborhoodByNameAsync(neighborhoodName);

                if (neighborhood is null)
                {
                    _logger.LogError("{Neighborhood} neighborhood dose not exists", neighborhoodName);
                    throw new InvalidArgumentException(Messages.InvalidNeighborhood);
                }
            }

            return (city, neighborhood);
        }

        private (TimeSpan, TimeSpan) ParseHours(string startHour, string finishHour)
        {
            string[] startHourArr = startHour.Split(':');
            string[] finishHourArr = finishHour.Split(':');

            TimeSpan startHourParsed = new (int.Parse(startHourArr[0]), int.Parse(startHourArr[1]), 0);

            TimeSpan finishHourParsed = new (int.Parse(finishHourArr[0]), int.Parse(finishHourArr[1]), 0);

            if (startHourParsed >= finishHourParsed) 
            {
                _logger.LogError("Start hour {StartHour} is larger or equal to finish hour {FinishHour}", startHour, finishHour);
                throw new InvalidArgumentException(Messages.StartHourIsLargerOrEqualToFinishHour);
            }

            return (startHourParsed, finishHourParsed);
        }
    }
}
