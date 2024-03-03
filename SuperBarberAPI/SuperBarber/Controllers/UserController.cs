using Business.Constants.Messages;
using Business.Interfaces;
using Business.Models.Exceptions;
using Business.Models.Requests;
using Business.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperBarber.Models;
using System.Net.Mime;
using System.Reflection;

namespace SuperBarber.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly string _controllerRouteTemplate;
        private readonly string _emailConfirmationRouteTemplate;
        
            
        public UserController(IUserService authenticationService, ILogger<UserController> logger)
        {
            _controllerRouteTemplate = GetType().GetCustomAttribute<RouteAttribute>()?.Template ??
                throw new NotConfiguredException(Messages.RouteTemplateNotConfigured);
            _emailConfirmationRouteTemplate = typeof(UserController).GetMethod(nameof(EmailConfirmation))?.GetCustomAttribute<RouteAttribute>()?.Template ??
                throw new NotConfiguredException(Messages.RouteTemplateNotConfigured);
            _userService = authenticationService;
            _logger = logger;
        }

        [HttpPost]
        [Route("register")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ResponseContent<AuthenticationResponse>), 200)]
        [ProducesDefaultResponseType(typeof(ResponseContent))]
        public async Task<ResponseContent<AuthenticationResponse>> Register([FromBody] UserRegisterRequest request)
        {
            AuthenticationResponse response = await _userService.RegisterUserAsync(request, _controllerRouteTemplate, _emailConfirmationRouteTemplate);

            return new ResponseContent<AuthenticationResponse>()
            {
                Result = response
            };
        }

        [HttpPost]
        [Route("login")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ResponseContent<AuthenticationResponse>), 200)]
        [ProducesDefaultResponseType(typeof(ResponseContent))]
        public async Task<ResponseContent<AuthenticationResponse>> Login([FromBody] UserLoginRequest request)
        {
            AuthenticationResponse response = await _userService.LoginUserAsync(request);

            return new ResponseContent<AuthenticationResponse>()
            {
                Result = response
            };
        }

        [Authorize]
        [HttpGet]
        [Route("email-confirmation")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ResponseContent<EmailConfirmationResponse>), 200)]
        [ProducesDefaultResponseType(typeof(ResponseContent))]
        public async Task<ResponseContent<EmailConfirmationResponse>> EmailConfirmation([FromQuery] EmailConfirmationRequest request) 
        {
            EmailConfirmationResponse response = await _userService.ConfirmEmailAsync(request);

            return new ResponseContent<EmailConfirmationResponse>()
            {
                Result = response
            };
        }
        
        [HttpPost]
        [Route("refresh-token")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ResponseContent<AuthenticationResponse>), 200)]
        [ProducesDefaultResponseType(typeof(ResponseContent))]
        public async Task<ResponseContent<AuthenticationResponse>> RefreshToken([FromBody] RefreshTokenRequest request) 
        {
            AuthenticationResponse response = await _userService.RefreshTokenAsync(request);

            return new ResponseContent<AuthenticationResponse>()
            {
                Result = response
            };
        }
    }
}
