using Business.Interfaces;
using Business.Models.Requests.BarberShop;
using Business.Models.Responses.BarberShop;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SuperBarber.Models;
using System.Net.Mime;

namespace SuperBarber.Controllers
{
    [Route("barber-shop")]
    [ApiController]
    public class BarberShopController : ControllerBase
    {
        private readonly IBarberShopService _barberShopService;
        private readonly ILogger<UserController> _logger;

        public BarberShopController(ILogger<UserController> logger, IBarberShopService barberShopService)
        {
            _logger = logger;
            _barberShopService = barberShopService;
        }

        [HttpGet]
        [Route("all")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ResponseContent<AllBarberShopsResponse>), 200)]
        [ProducesDefaultResponseType(typeof(ResponseContent))]
        public async Task<ResponseContent<AllBarberShopsResponse>> GetAll([FromQuery] AllBarberShopRequest request)
        {
            AllBarberShopsResponse response = await _barberShopService.GetAllBarberShopsAsync(request);

            return new ResponseContent<AllBarberShopsResponse>()
            {
                Result = response
            };
        }
        
        [HttpPost]
        [Route("register")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ResponseContent<RegisterBarberShopResponse>), 200)]
        [ProducesDefaultResponseType(typeof(ResponseContent))]
        public async Task<ResponseContent<RegisterBarberShopResponse>> Register(RegisterBarberShopRequest request)
        {
            RegisterBarberShopResponse response = await _barberShopService.RegisterBarberShopAsync(request);

            return new ResponseContent<RegisterBarberShopResponse>()
            {
                Result = response
            };
        }
        
        [HttpPatch]
        [Route("update/{id}")]
        [Consumes(MediaTypeNames.Application.JsonPatch)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ResponseContent<UpdateBarberShopResponse>), 200)]
        [ProducesDefaultResponseType(typeof(ResponseContent))]
        public async Task<ResponseContent<UpdateBarberShopResponse>> Update(
            [FromRoute] int id, 
            [FromBody] JsonPatchDocument<UpdateBarberShopRequest> patchDoc)
        {
            UpdateBarberShopResponse response = await _barberShopService.UpdateBarberShopAsync(id, patchDoc);

            return new ResponseContent<UpdateBarberShopResponse>()
            {
                Result = response
            };
        }
    }
}
