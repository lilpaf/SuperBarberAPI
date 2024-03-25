using Business.Interfaces;
using Business.Models.Requests.Barber;
using Business.Models.Responses.Barber;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperBarber.Models;
using System.Net.Mime;

namespace SuperBarber.Controllers
{
    [Route("barber")]
    [ApiController]
    public class BarberController : ControllerBase
    {
        private readonly ILogger<BarberController> _logger;
        private readonly IBarberService _barberService;

        public BarberController(ILogger<BarberController> logger, IBarberService barberService)
        {
            _logger = logger;
            _barberService = barberService;
        }

        [Authorize]
        [HttpPost]
        [Route("register")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ResponseContent<RegisterBarberResponse>), 200)]
        [ProducesDefaultResponseType(typeof(ResponseContent))]
        public async Task<ResponseContent<RegisterBarberResponse>> Register([FromBody] RegisterBarberRequest request)
        {
            RegisterBarberResponse response = await _barberService.RegisterBarberAsync(request);

            return new ResponseContent<RegisterBarberResponse>()
            {
                Result = response
            };
        }

        [HttpGet]
        [Route("all")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ResponseContent<AllBarbersResponse>), 200)]
        [ProducesDefaultResponseType(typeof(ResponseContent))]
        public async Task<ResponseContent<AllBarbersResponse>> Register([FromQuery] AllBarbersRequest request)
        {
            AllBarbersResponse response = await _barberService.GetAllAsync(request);

            return new ResponseContent<AllBarbersResponse>()
            {
                Result = response
            };
        }

    }
}
