using Business.Interfaces;
using Business.Models.Dtos;
using Business.Models.Exceptions;
using Common.Constants;
using Microsoft.Extensions.Logging;
using Persistence.Entities;
using Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Business.Implementations
{
    public class CityService : ICityService
    {
        private readonly ILogger<CityService> _logger;
        private readonly IRedisRepository _redisRepository;
        private readonly ICityRepository _cityRepository;
        private readonly INeighborhoodRepository _neighborhoodRepository;

        public CityService(IRedisRepository redisRepository, INeighborhoodRepository neighborhoodRepository, ICityRepository cityRepository, ILogger<CityService> logger)
        {
            _redisRepository = redisRepository;
            _neighborhoodRepository = neighborhoodRepository;
            _cityRepository = cityRepository;
            _logger = logger;
        }


        public async Task<IReadOnlyList<CityDto>> GetCitiesAndNeighborhoodsAsync()
        {
            string? citiesJson = await _redisRepository.GetDataAsync(RedisConstants.CitiesKeyRedis);
            IReadOnlyList<CityDto>? cachedCities = null;

            if (citiesJson is not null)
            {
                cachedCities = DeserializeCities(citiesJson);
            }

            if (cachedCities is not null)
            {
                return cachedCities;
            }

            List<CityDto> cityDtos = new();

            IReadOnlyList<City> cities = await _cityRepository.GetAllCitiesAsync();

            foreach (var city in cities)
            {
                IReadOnlyList<Neighborhood> neighborhoods = await _neighborhoodRepository
                    .GetAllNeighborhoodsByCityIdAsync(city.Id);

                if (!neighborhoods.Any())
                {
                    cityDtos.Add(new CityDto
                    {
                        Id = city.Id,
                        Name = city.Name,
                        Neighborhoods = null
                    });

                    continue;
                }

                cityDtos.Add(new CityDto
                {
                    Id = city.Id,
                    Name = city.Name,
                    Neighborhoods = neighborhoods
                        .Select(neighborhood => new NeighborhoodDto
                        {
                            Id = neighborhood.Id,
                            Name = neighborhood.Name
                        })
                        .ToList()
                });
            }

            string json = SerializeCities(cityDtos);

            await _redisRepository.SaveDataAsync(RedisConstants.CitiesKeyRedis ,json);

            return cityDtos;
        }

        private IReadOnlyList<CityDto>? DeserializeCities(string json)
        {
            try
            {
                IReadOnlyList<CityDto>? cities = JsonSerializer.Deserialize<IReadOnlyList<CityDto>>(json);
                return cities;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred when deserializing city data. Error: {Error}", ex);
                return null;
            }
        }
        
        private string SerializeCities(IReadOnlyList<CityDto> cities)
        {
            return JsonSerializer.Serialize(cities, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder =  JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
            });
        }
    }
}
