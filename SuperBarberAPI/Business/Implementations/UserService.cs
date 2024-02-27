using Business.Configurations;
using Business.Constants;
using Business.Constants.Messages;
using Business.Interfaces;
using Business.Models.Exceptions;
using Business.Models.Requests;
using Business.Models.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Persistence.Entities;
using Persistence.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly JwtConfig _jwtConfig;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<User> userManager,
            ILogger<UserService> logger,
            IOptions<JwtConfig> jwtConfig,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _logger = logger;
            _jwtConfig = jwtConfig.Value;
            _userRepository = userRepository;
        }

        public async Task<AuthenticationResponse> RegisterUser(UserRegisterRequest request)
        {
            bool userExists = await _userRepository.FindUserByEmailAsync(request.Email);

            if (userExists) 
            {
                _logger.LogError("User with this {email} already exists", request.Email);
                throw new InvalidArgumentException(String.Format(Messages.UserExists, request.Email));
            }

            User user = new()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };

            IdentityResult isUserCreated = await _userManager.CreateAsync(user, request.Password);

            if (!isUserCreated.Succeeded)
            {
                string[] errorMessages = isUserCreated.Errors.Select(e => e.Description).ToArray();
                
                _logger.LogError("There was an error when creating the user. Error messages: {errorMessages}", String.Join(ErrorConstants.ErrorDelimiter, errorMessages));
                throw new ErrorCreatingUserException(Messages.ErrorCreatingUser);
            }

            string token = GenerateJwtToken(user);

            return new AuthenticationResponse()
            {
                Token = token
            };
        }

        public Task<AuthenticationResponse> LoginUser(UserLoginRequest request)
        {
            throw new NotImplementedException();
        }

        private string GenerateJwtToken(User user)
        {
            JwtSecurityTokenHandler jwtTokenHandler = new();

            byte[] key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                }),

                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            SecurityToken token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}
