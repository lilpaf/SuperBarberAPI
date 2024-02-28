using Business.Configurations;
using Business.Constants;
using Business.Constants.Messages;
using Business.Interfaces;
using Business.Models.Exceptions;
using Business.Models.Requests;
using Business.Models.Responses;
using Microsoft.AspNetCore.Http;
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
        private readonly IEmailService _emailService;

        public UserService(
            UserManager<User> userManager,
            ILogger<UserService> logger,
            IOptions<JwtConfig> jwtConfig,
            IUserRepository userRepository,
            IEmailService emailService)
        {
            _userManager = userManager;
            _logger = logger;
            _jwtConfig = jwtConfig.Value;
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<AuthenticationResponse> RegisterUserAsync(UserRegisterRequest request, string routeTemplate, string emailConformationAction)
        {
            bool userExists = await _userRepository.UserExistsByEmailAsync(request.Email);

            if (userExists) 
            {
                _logger.LogError("User with this {email} already exists", request.Email);
                throw new InvalidArgumentException(Messages.UserExists);
            }

            User user = new()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = false
            };

            IdentityResult isUserCreated = await _userManager.CreateAsync(user, request.Password);

            if (!isUserCreated.Succeeded)
            {
                string[] errorMessages = isUserCreated.Errors.Select(e => e.Description).ToArray();
                
                _logger.LogError("There was an error when creating the user. Error messages: {errorMessages}", String.Join(ErrorConstants.ErrorDelimiter, errorMessages));
                throw new ErrorCreatingUserException(Messages.ErrorCreatingUser);
            }

            //ToDo email conformation

            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            await _emailService.SendConformationEmail(routeTemplate, emailConformationAction, user, code);

            string jwtToken = GenerateJwtToken(user);

            return new AuthenticationResponse()
            {
                Token = jwtToken
            };
        }

        public async Task<AuthenticationResponse> LoginUserAsync(UserLoginRequest request)
        {
            User? user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                _logger.LogError("User with this {email} dose not exists", request.Email);
                throw new InvalidArgumentException(Messages.WrongCredentials);
            }

            bool isCorrect = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isCorrect)
            {
                _logger.LogError("Wrong password provided for user");
                throw new InvalidArgumentException(Messages.WrongCredentials);
            }

            string jwtToken = GenerateJwtToken(user);

            return new AuthenticationResponse() 
            { 
                Token = jwtToken
            };
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

                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            SecurityToken token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}
