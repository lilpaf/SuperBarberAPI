using Business.Interfaces;
using Business.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace SuperBarber.Controllers
{
    [Route("authentication")]
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
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            await _userService.RegisterUser(request);
            return Ok();
        }
    }
}
