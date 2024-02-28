using Business.Interfaces;
using Business.Models.Requests;
using Business.Models.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SuperBarber.Models;
using System.Net.Mime;

namespace SuperBarber.Controllers
{
    [Route("barber_shop")]
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

        [Authorize] //ToDo remove it 
        [HttpGet]
        [Route("all")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ResponseContent<AllBarberShopsResponse>), 200)]
        [ProducesDefaultResponseType(typeof(ResponseContent))]
        public async Task<ResponseContent<AllBarberShopsResponse>> Register()
        {
            AllBarberShopsResponse response = await _barberShopService.GetAllBarberShopsAsync();

            return new ResponseContent<AllBarberShopsResponse>()
            {
                Result = response
            };
        }
    }
}
