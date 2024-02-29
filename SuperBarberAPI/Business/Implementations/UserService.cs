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
        private readonly SignInManager<User> _signInManager;
        private readonly IUserRepository _userRepository;
        private readonly JwtConfig _jwtConfig;
        private readonly ILogger<UserService> _logger;
        private readonly IEmailService _emailService;

        public UserService(
            UserManager<User> userManager,
            ILogger<UserService> logger,
            IOptions<JwtConfig> jwtConfig,
            IUserRepository userRepository,
            IEmailService emailService,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _logger = logger;
            _jwtConfig = jwtConfig.Value;
            _userRepository = userRepository;
            _emailService = emailService;
            _signInManager = signInManager;
        }

        public async Task<AuthenticationResponse> RegisterUserAsync(UserRegisterRequest request, string controllerRouteTemplate, string emailConformationRouteTemplate)
        {
            bool userExists = await _userRepository.UserIsActiveAndExistsByEmailAsync(request.Email);

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
                string[] errorsMessages = isUserCreated.Errors.Select(e => e.Description).ToArray();

                _logger.LogError("There was an error when creating the user. Error messages: {errorMessages}", String.Join(ErrorConstants.ErrorDelimiter, errorsMessages));
                throw new ErrorCreatingUserException(Messages.ErrorCreatingUser);
            }

            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            await _emailService.SendConformationEmail(controllerRouteTemplate, emailConformationRouteTemplate, user, code);

            string jwtToken = GenerateJwtToken(user);

            return new AuthenticationResponse()
            {
                Token = jwtToken
            };
        }

        public async Task<AuthenticationResponse> LoginUserAsync(UserLoginRequest request)
        {
            User? user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null || user.IsDeleted)
            {
                _logger.LogError("User with this email {email} dose not exists", request.Email);
                throw new InvalidArgumentException(Messages.WrongCredentials);
            }

            SignInResult result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, request.RememberMe, true);

            if (result.IsLockedOut)
            {
                _logger.LogError("User account locked out");
                throw new InvalidArgumentException(Messages.UserIsLockedOut);
            }
            if (!result.Succeeded)
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
        
        public async Task<EmailConfirmationResponse> ConfirmEmailAsync(EmailConfirmationRequest request)
        {
            User? user = await _userManager.FindByIdAsync(request.UserId);

            if (user is null || user.IsDeleted)
            {
                _logger.LogError("User with this id {id} dose not exists", request.UserId);
                throw new InvalidArgumentException(Messages.UserDoesNotExist);
            }
            
            // Decodes the token since when it is html encoded
            string code = request.Code.Replace(" ", "+");

            IdentityResult emailIsConfirmed = await _userManager.ConfirmEmailAsync(user, code);
           
            string message = emailIsConfirmed.Succeeded ? Messages.EmailConfirmationSucceeded : Messages.ErrorConfirmingEmail;

            if (!emailIsConfirmed.Succeeded)
            {
                string[] errorsMessages = emailIsConfirmed.Errors.Select(e => e.Description).ToArray();

                _logger.LogError("There was an error when confirming the user {userId} email {userEmail}. Error messages: {errorMessages}", 
                    user.Id, user.Email, String.Join(ErrorConstants.ErrorDelimiter, errorsMessages));
                throw new ErrorCreatingUserException(Messages.ErrorConfirmingEmail);
            }

            return new EmailConfirmationResponse()
            { 
                Message = message
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
