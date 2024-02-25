using Business.Configurations;
using Business.Interfaces;
using Business.Models.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Persistence.Entities;
using Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IValidatorService _validatorService;
        private readonly IUserRepository _userRepository;
        private readonly JWTConfig _jwtConfig;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<User> userManager,
            ILogger<UserService> logger,
            JWTConfig jwtConfig,
            IValidatorService validatorService,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _logger = logger;
            _jwtConfig = jwtConfig;
            _validatorService = validatorService;
            _userRepository = userRepository;
        }

        public async Task RegisterUser(RegisterRequest request)
        {
            _validatorService.Validate(request);

            User user = new()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = request.Password // ToDo fix it
            };

            await _userRepository.AddUserAsync(user);

            await _userRepository.SaveUserAsync();
        }
    }
}
