using Business.Interfaces;
using Business.Models.Dtos;
using Business.Models.Exceptions;
using Business.Models.Requests.BarberShop;
using Business.Models.Responses.BarberShop;
using Common.Constants.Messages;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Persistence.Entities;
using Persistence.Interfaces;
using Persistence.Models;
using System.ComponentModel.DataAnnotations;

namespace Business.Implementations
{
    public class BarberShopService : IBarberShopService
    {
        private const string StringDateTimeFormat = @"hh\:mm";
        private readonly IBarberShopRepository _barberShopRepository;
        private readonly ICityRepository _cityRepository;
        private readonly INeighborhoodRepository _neighborhoodRepository;
        private readonly IWeekDayRepository _weekDayRepository;
        private readonly ILogger<BarberShopService> _logger;

        public BarberShopService(
            ILogger<BarberShopService> logger,
            IBarberShopRepository barberShopRepository,
            ICityRepository cityRepository,
            INeighborhoodRepository neighborhoodRepository,
            IWeekDayRepository weekDayRepository)
        {
            _logger = logger;
            _barberShopRepository = barberShopRepository;
            _cityRepository = cityRepository;
            _neighborhoodRepository = neighborhoodRepository;
            _weekDayRepository = weekDayRepository;
        }

        public async Task<AllBarberShopsResponse> GetAllPublicBarberShopsAsync(AllBarberShopRequest request)
        {
            QueryParameterContainer queryParams = new()
            {
                City = request.City,
                Neighborhood = request.Neighborhood,
                SearchName = request.BarberShopName
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

            IReadOnlyList<string> neighborhoodsName = await _neighborhoodRepository
                .GetAllNeighborhoodsNameByCityNameFromRedisAsync(city.Name);

            if (!neighborhoodsName.Any())
            {
                IReadOnlyList<Neighborhood> neighborhoods = await _neighborhoodRepository
                    .GetAllNeighborhoodsByCityIdAsync(city.Id);

                neighborhoodsName = neighborhoods.Select(n => n.Name).ToList();
            }

            IReadOnlyList<BarberShop> publicBarberShops = await _barberShopRepository
                .GetAllPublicBarberShopsWithCitiesNeighborhoodsAndWorkingDaysAsync(queryParams);

            IReadOnlyList<AllBarberShopDto> publicBarberShopsDto = publicBarberShops
                .Select(b => new AllBarberShopDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Address = b.Address,
                    WorkingWeekHoursToday = GetBarberShopWorkingHoursTodayUtc(b.BarberShopWorkingDays),
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
                BarberShopSearchName = request.BarberShopName,
                //TotalPages = totalActiveBarberShops / QueryParameterContainer.BarberShopsPerPage, //ToDo may be needed
                BarberShops = publicBarberShopsDto
            };
        }

        public async Task<BarberShopResponse> GetPublicBarberShopAsync(int barberShopId)
        {
            BarberShop? publicBarberShop = await _barberShopRepository
                .GetOnlyPublicBarberShopWithCitiesNeighborhoodsAndWorkingDaysByIdAsync(barberShopId);

            if (publicBarberShop is null)
            {
                _logger.LogError("Barber shop with this id {Id} dose not exists", barberShopId);
                throw new InvalidArgumentException(Messages.BarberShopDoseNotExist);
            }

            //Dictionary<string, string?> workingWeekHours = GetBarberShopWorkingWeekHours(publicBarberShop);
            Dictionary<string, Tuple<string?, string?>> workingWeekHours = GetWorkingDaysHours(publicBarberShop.BarberShopWorkingDays);

            return new BarberShopResponse()
            {
                Id = publicBarberShop.Id,
                Name = publicBarberShop.Name,
                About = publicBarberShop.About,
                Address = publicBarberShop.Address,
                AverageRating = publicBarberShop.AverageRating,
                WorkingWeekHours = workingWeekHours,
            };
        }

        public async Task<RegisterBarberShopResponse> RegisterBarberShopAsync(RegisterBarberShopRequest request)
        {
            City? city;
            Neighborhood? neighborhood;

            (city, neighborhood) = await GetCityAndNeighborhoodByName(request.City, request.Neighborhood);

            HashSet<BarberShopWorkingDay> workingDays = await SetWorkingDaysHoursAsync(request.WorkingDaysHours);

            BarberShop barberShop = new()
            {
                Name = request.Name,
                CityId = city.Id,
                Neighborhood = neighborhood,
                Address = request.Address,
                About = request.About,
                IsPublic = false,
                IsDeleted = false,
                AverageRating = 0,
                BarberShopWorkingDays = workingDays
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
            BarberShop? barberShopFromRepo = await _barberShopRepository
                .GetPublicAndPrivateBarberShopWithCitiesNeighborhoodsAndWorkingDaysByIdAsync(barberShopId);

            if (barberShopFromRepo is null)
            {
                _logger.LogError("Barber shop with this id {Id} dose not exists", barberShopId);
                throw new InvalidArgumentException(Messages.BarberShopDoseNotExist);
            }

            Dictionary<string, Tuple<string?, string?>> workingDays = GetWorkingDaysHours(barberShopFromRepo.BarberShopWorkingDays);

            UpdateBarberShopRequest updatedBarberShop = new()
            {
                Name = barberShopFromRepo.Name,
                City = barberShopFromRepo.City.Name,
                Neighborhood = barberShopFromRepo.Neighborhood?.Name,
                Address = barberShopFromRepo.Address,
                About = barberShopFromRepo.About,
                WorkingDaysHours = workingDays
            };

            patchDoc.ApplyTo(updatedBarberShop);

            List<ValidationResult> validationResults = new();
            ValidationContext validationContext = new(updatedBarberShop, null, null);

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

            HashSet<BarberShopWorkingDay> workingDaysUpdated = await SetWorkingDaysHoursAsync(updatedBarberShop.WorkingDaysHours);

            barberShopFromRepo.Name = updatedBarberShop.Name;
            barberShopFromRepo.City = city;
            barberShopFromRepo.Neighborhood = neighborhood;
            barberShopFromRepo.Address = updatedBarberShop.Address;
            barberShopFromRepo.BarberShopWorkingDays = workingDaysUpdated;
            barberShopFromRepo.IsPublic = false;

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

            TimeSpan startHourParsed = new(int.Parse(startHourArr[0]), int.Parse(startHourArr[1]), 0);

            TimeSpan finishHourParsed = new(int.Parse(finishHourArr[0]), int.Parse(finishHourArr[1]), 0);

            if (startHourParsed >= finishHourParsed)
            {
                _logger.LogError("Start hour {StartHour} is larger or equal to finish hour {FinishHour}",
                    startHourParsed.ToString(), finishHourParsed.ToString());
                throw new InvalidArgumentException(Messages.StartHourIsLargerOrEqualToFinishHour);
            }

            return (startHourParsed, finishHourParsed);
        }

        private Dictionary<string, Tuple<string?, string?>> GetBarberShopWorkingHoursTodayUtc(ICollection<BarberShopWorkingDay> barberShopWorkingDays)
        {
            BarberShopWorkingDay today = barberShopWorkingDays
                .First(b => b.WeekDay.DayOfWeekEnum == DateTime.UtcNow.DayOfWeek);

            string dayOfWeek = today.WeekDay.DayOfWeekName;
            string? startHour = today.OpeningHour?.ToString();
            string? finishHour = today.ClosingHour?.ToString();

            return new Dictionary<string, Tuple<string?, string?>>()
            {
                { dayOfWeek, Tuple.Create(startHour, finishHour) }
            };
        }

        private Dictionary<string, Tuple<string?, string?>> GetWorkingDaysHours(ICollection<BarberShopWorkingDay> workingDays)
        {
            Dictionary<string, Tuple<string?, string?>> result = new();
            
            foreach (var barberShopWorkingDay in workingDays)
            {
                string weekDayName = barberShopWorkingDay.WeekDay.DayOfWeekName;

                string? openingHour = barberShopWorkingDay.OpeningHour?.ToString(StringDateTimeFormat);
                string? closingHour = barberShopWorkingDay.ClosingHour?.ToString(StringDateTimeFormat);

                result.Add(weekDayName, Tuple.Create(openingHour, closingHour));
            }

            return result;
        }

        private async Task<HashSet<BarberShopWorkingDay>> SetWorkingDaysHoursAsync(Dictionary<string, Tuple<string?, string?>> workingDaysHours)
        {
            HashSet<BarberShopWorkingDay> result = new();

            foreach (var workingDays in workingDaysHours)
            {
                WeekDay? weekDay = await _weekDayRepository.GetWeekDayByDayNameAsync(workingDays.Key);

                if (weekDay is null)
                {
                    _logger.LogError("Week day dose not exists with this week day name {Id}", workingDays.Key);
                    throw new InvalidArgumentException(Messages.InvalidAndDateHourFormat);
                }

                string? openingHour = workingDays.Value.Item1;
                string? closingHour = workingDays.Value.Item2;

                if (openingHour is null || closingHour is null)
                {
                    result.Add(new BarberShopWorkingDay
                    {
                        WeekDayId = weekDay.Id,
                        OpeningHour = null,
                        ClosingHour = null
                    });

                    continue;
                }

                TimeSpan openingHourParsed;
                TimeSpan closingHourParsed;

                (openingHourParsed, closingHourParsed) = ParseHours(openingHour, closingHour);

                result.Add(new BarberShopWorkingDay
                {
                    WeekDayId = weekDay.Id,
                    OpeningHour = openingHourParsed,
                    ClosingHour = closingHourParsed
                });
            }

            return result;
        }
    }
}
