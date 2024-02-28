﻿using Business.Interfaces;
using Business.Models.Requests;
using Business.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using SuperBarber.Models;
using System.Net.Mime;

namespace SuperBarber.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService authenticationService, ILogger<UserController> logger)
        {
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
            AuthenticationResponse response = await _userService.RegisterUserAsync(request);
            
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
    }
}
