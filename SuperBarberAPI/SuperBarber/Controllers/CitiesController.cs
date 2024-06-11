using Business.Interfaces;
using Business.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using SuperBarber.Models;
using System.Net.Mime;

namespace SuperBarber.Controllers
{
    [Route("cities")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CitiesController(ICityService cityRepository)
        {
            _cityService = cityRepository;
        }

        [HttpGet]
        [Route("all")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ResponseContent<IReadOnlyList<CityDto>>), 200)]
        [ProducesDefaultResponseType(typeof(ResponseContent))]
        public async Task<ResponseContent<IReadOnlyList<CityDto>>> GetAll()
        {
            IReadOnlyList<CityDto> response = await _cityService.GetCitiesAndNeighborhoodsAsync();

            return new ResponseContent<IReadOnlyList<CityDto>>()
            {
                Result = response
            };
        }
    }
}
