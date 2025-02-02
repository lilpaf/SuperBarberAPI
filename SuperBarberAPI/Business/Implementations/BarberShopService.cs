using Business.Interfaces;
using Business.Models.Dtos;
using Business.Models.Exceptions;
using Business.Models.Requests.BarberShop;
using Business.Models.Responses.BarberShop;
using Common.Constants;
using Common.Constants.Resourses;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Persistence.Entities;
using Persistence.Interfaces;
using Persistence.Models;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Business.Implementations
{
    public class BarberShopService : IBarberShopService
    {
        private const string OpeningAndClosingHourFormat = @"hh\:mm";
        private readonly IBarberShopRepository _barberShopRepository;
        private readonly ICityRepository _cityRepository;
        private readonly INeighborhoodRepository _neighborhoodRepository;
        private readonly IWeekDayRepository _weekDayRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BarberShopService> _logger;

        public BarberShopService(
            ILogger<BarberShopService> logger,
            IBarberShopRepository barberShopRepository,
            ICityRepository cityRepository,
            INeighborhoodRepository neighborhoodRepository,
            IWeekDayRepository weekDayRepository,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _barberShopRepository = barberShopRepository;
            _cityRepository = cityRepository;
            _neighborhoodRepository = neighborhoodRepository;
            _weekDayRepository = weekDayRepository;
            _serviceProvider = serviceProvider;
        }

        public async Task<AllBarberShopsResponse> GetAllPublicBarberShopsAsync(AllBarberShopRequest request)
        {
            //ToDo may be needed if not delete the method
            //int totalActiveBarberShops = await _barberShopRepository.GetTotalNumberActiveBarberShopsAsync();

            City? city = await _cityRepository.GetCityByNameAsync(request.City);

            if (city is null)
            {
                _logger.LogError("{City} city dose not exists", request.City);
                throw new InvalidArgumentException(Messages.InvalidCity);
            }

            DateTime requestedDate = DateTime.ParseExact(request.Date, DataConstraints.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);

            QueryParameterContainer queryParams = new()
            {
                City = request.City,
                Neighborhood = request.Neighborhood,
                SearchName = request.BarberShopName,
                Date = requestedDate,
            };

            IReadOnlyList<BarberShop> publicBarberShops = await _barberShopRepository
                .GetAllPublicBarberShopsWithCitiesNeighborhoodsAndWorkingDaysAsync(queryParams);

            IReadOnlyList<AllBarberShopDto> publicBarberShopsDto = publicBarberShops
                .Select(b => new AllBarberShopDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Address = b.Address,
                    AverageRating = b.AverageRating,
                    //ToDo fix it
                    //ImageName = b.ImageName
                })
                .ToList();

            return new AllBarberShopsResponse()
            {
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
            Dictionary<string, DayHoursDto> workingWeekHours = GetWorkingDaysHours(publicBarberShop.BarberShopWorkingDays);

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

            Dictionary<string, DayHoursDto> workingDays = GetWorkingDaysHours(barberShopFromRepo.BarberShopWorkingDays);

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
            ValidationContext validationContext = new(updatedBarberShop, _serviceProvider, null);

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

        private Dictionary<string, DayHoursDto> GetWorkingDaysHours(ICollection<BarberShopWorkingDay> workingDays)
        {
            Dictionary<string, DayHoursDto> result = new();
            
            foreach (var barberShopWorkingDay in workingDays)
            {
                string weekDayName = barberShopWorkingDay.WeekDay.DayOfWeekName;

                string? openingTime = barberShopWorkingDay.OpeningHour?.ToString(OpeningAndClosingHourFormat);
                string? closingTime = barberShopWorkingDay.ClosingHour?.ToString(OpeningAndClosingHourFormat);

                result.Add(weekDayName, new DayHoursDto() { OpeningTime = openingTime, ClosingTime = closingTime });
            }

            return result;
        }

        private async Task<HashSet<BarberShopWorkingDay>> SetWorkingDaysHoursAsync(Dictionary<string, DayHoursDto> workingDaysHours)
        {
            HashSet<BarberShopWorkingDay> result = new();

            foreach (var workingDays in workingDaysHours)
            {
                WeekDay? weekDay = await _weekDayRepository.GetWeekDayByDayNameAsync(workingDays.Key);

                if (weekDay is null)
                {
                    _logger.LogError("Week day dose not exists with this week day name {Id}", workingDays.Key);
                    throw new InvalidArgumentException(Messages.InvalidDateOrHourFormat);
                }

                string? openingHour = workingDays.Value.OpeningTime;
                string? closingHour = workingDays.Value.ClosingTime;

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
