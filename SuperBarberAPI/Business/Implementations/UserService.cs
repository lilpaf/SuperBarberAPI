using Business.Configurations;
using Business.Constants;
using Business.Constants.Messages;
using Business.Helpers;
using Business.Interfaces;
using Business.Models.Exceptions;
using Business.Models.Requests;
using Business.Models.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
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
        private readonly TokenValidationParameters _tokenValidationParameters;

        public UserService(
            UserManager<User> userManager,
            ILogger<UserService> logger,
            IOptions<JwtConfig> jwtConfig,
            IUserRepository userRepository,
            IEmailService emailService,
            SignInManager<User> signInManager,
            TokenValidationParameters tokenValidationParameters)
        {
            _userManager = userManager;
            _logger = logger;
            _jwtConfig = jwtConfig.Value;
            _userRepository = userRepository;
            _emailService = emailService;
            _signInManager = signInManager;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task<AuthenticationResponse> RegisterUserAsync(UserRegisterRequest request, string controllerRouteTemplate, string emailConformationRouteTemplate)
        {
            bool userExists = await _userRepository.UserIsActiveAndExistsByEmailAsync(request.Email);

            if (userExists)
            {
                _logger.LogError("User with this {Email} already exists", request.Email);
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

                _logger.LogError("There was an error when creating the user. Error messages: {ErrorMessages}", String.Join(ErrorConstants.ErrorDelimiter, errorsMessages));
                throw new ErrorCreatingUserException(Messages.ErrorCreatingUser);
            }

            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            //Encode the code
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            await _emailService.SendConformationEmail(controllerRouteTemplate, emailConformationRouteTemplate, user, code);

            AuthenticationResponse response = await GenerateJwtTokenAsync(user);

            return response;
        }

        public async Task<AuthenticationResponse> LoginUserAsync(UserLoginRequest request)
        {
            User? user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null || user.IsDeleted)
            {
                _logger.LogError("User with this email {Email} dose not exists", request.Email);
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

            AuthenticationResponse response = await GenerateJwtTokenAsync(user);

            return response;
        }

        public async Task<EmailConfirmationResponse> ConfirmEmailAsync(EmailConfirmationRequest request)
        {
            User? user = await _userManager.FindByIdAsync(request.UserId);

            if (user is null || user.IsDeleted)
            {
                _logger.LogError("User with this id {Id} dose not exists", request.UserId);
                throw new InvalidArgumentException(Messages.UserDoesNotExist);
            }

            //Decode the code
            string code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));

            IdentityResult emailIsConfirmed = await _userManager.ConfirmEmailAsync(user, code);

            string message = emailIsConfirmed.Succeeded ? Messages.EmailConfirmationSucceeded : Messages.ErrorConfirmingEmail;

            if (!emailIsConfirmed.Succeeded)
            {
                string[] errorsMessages = emailIsConfirmed.Errors.Select(e => e.Description).ToArray();

                _logger.LogError("There was an error when confirming the user {UserId} email {UserEmail}. Error messages: {ErrorMessages}",
                    user.Id, user.Email, String.Join(ErrorConstants.ErrorDelimiter, errorsMessages));
                throw new ErrorCreatingUserException(Messages.ErrorConfirmingEmail);
            }

            return new EmailConfirmationResponse()
            {
                Message = message
            };
        }

        public async Task<AuthenticationResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            JwtSecurityTokenHandler jwtTokenHandler = new();

            TokenValidationResult tokenVerificationResult = await jwtTokenHandler.ValidateTokenAsync(request.Token, _tokenValidationParameters);

            if (!tokenVerificationResult.IsValid)
            {
                _logger.LogError("Token verification was unsuccessful for token: {Token}. Error: {Error}",
                    request.Token, tokenVerificationResult.Exception.Message);
                throw new InvalidArgumentException(Messages.InvalidJwtToken);
            }

            SecurityToken validatedToken = tokenVerificationResult.SecurityToken;

            if (validatedToken.ValidTo < DateTime.UtcNow)
            {
                _logger.LogError("The validated token {Token} has expired", request.Token);
                throw new InvalidArgumentException(Messages.InvalidJwtToken);
            }

            UserRefreshToken? userRefreshToken = await _userRepository.GetUserRefreshTokenWithUserByTokenAsync(request.RefreshToken);

            if (userRefreshToken is null)
            {
                _logger.LogError("No refresh token exists that matches this token {Token}", request.Token);
                throw new InvalidArgumentException(Messages.InvalidJwtToken);
            }

            if (userRefreshToken.IsUsed || userRefreshToken.IsRevoked)
            {
                _logger.LogError("Refresh token {TokenId} has been used or is revoked", userRefreshToken.Id);
                throw new InvalidArgumentException(Messages.InvalidJwtToken);
            }

            string? jti = tokenVerificationResult.Claims.FirstOrDefault(c => c.Key == JwtRegisteredClaimNames.Jti).Value.ToString();

            if (userRefreshToken.JwtId != jti)
            {
                _logger.LogError("Refresh token {TokenId} has invalid JwtId", userRefreshToken.Id);
                throw new InvalidArgumentException(Messages.InvalidJwtToken);
            }

            if (userRefreshToken.ExpiryDate < DateTime.UtcNow)
            {
                _logger.LogError("The refresh token {TokenId} has expired", userRefreshToken.Id);
                throw new InvalidArgumentException(Messages.InvalidJwtToken);
            }

            userRefreshToken.IsUsed = true;

            _userRepository.UpdateUserRefreshToken(userRefreshToken);
            await _userRepository.SaveChangesAsync();

            User user = userRefreshToken.User;

            AuthenticationResponse response = await GenerateJwtTokenAsync(user);

            return response;
        }

        private async Task<AuthenticationResponse> GenerateJwtTokenAsync(User user)
        {
            JwtSecurityTokenHandler jwtTokenHandler = new();

            byte[] key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("user_id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                }),

                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience,
                Expires = DateTime.UtcNow.AddHours(_jwtConfig.AccessTokenHoursLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            SecurityToken token = jwtTokenHandler.CreateToken(tokenDescriptor);
            string jwtToken = jwtTokenHandler.WriteToken(token);

            UserRefreshToken? userRefreshToken = await _userRepository.GetUserRefreshTokenByUserIdAsync(user.Id);

            if (userRefreshToken is null)
            {
                userRefreshToken = new()
                {
                    JwtId = token.Id,
                    Token = StringHelper.RandomStringGenerator(_jwtConfig.RefreshTokenLength),
                    CreatedDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddHours(_jwtConfig.RefreshTokenHoursLifetime),
                    IsRevoked = false,
                    IsUsed = false,
                    UserId = user.Id
                };

                await _userRepository.AddUserRefreshTokenAsync(userRefreshToken);
            }
            else
            {
                userRefreshToken.JwtId = token.Id;
                userRefreshToken.Token = StringHelper.RandomStringGenerator(_jwtConfig.RefreshTokenLength);
                userRefreshToken.CreatedDate = DateTime.UtcNow;
                userRefreshToken.ExpiryDate = DateTime.UtcNow.AddHours(_jwtConfig.RefreshTokenHoursLifetime);
                userRefreshToken.IsRevoked = false;
                userRefreshToken.IsUsed = false;

                _userRepository.UpdateUserRefreshToken(userRefreshToken);
            }

            await _userRepository.SaveChangesAsync();

            return new AuthenticationResponse()
            {
                Token = jwtToken,
                RefreshToken = userRefreshToken.Token
            };
        }


    }
}
