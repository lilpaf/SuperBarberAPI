using Azure;
using Business.Configurations;
using Business.Constants;
using Business.Constants.Messages;
using Business.Interfaces;
using Business.Models.Exceptions;
using Business.Models.Requests;
using Business.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Persistence.Entities;
using Persistence.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
        private static HttpContext _httpContext => new HttpContextAccessor().HttpContext ?? 
            throw new NotConfiguredException(Messages.NoActiveHttpContext);
        private static CookieOptions _cookieOptions => new() { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true };

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
            response.Message = Messages.UserRegistrationSussesfuly;

            return response;
        }

        public async Task<AuthenticationResponse> LoginUserAsync(UserLoginRequest request)
        {
            User? user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null || user.IsDeleted)
            {
                _logger.LogError("User with this email {Email} dose not exists or is deleted", request.Email);
                throw new InvalidArgumentException(Messages.WrongCredentials);
            }

            SignInResult result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, true);

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
            response.Message = Messages.LogInSussesfuly;

            return response;
        }

        public async Task<EmailConfirmationResponse> ConfirmEmailAsync(EmailConfirmationRequest request)
        {
            User user = await GetUserByUserClaimIdAsync();

            //Decode the code
            string code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));

            IdentityResult emailIsConfirmed = await _userManager.ConfirmEmailAsync(user, code);

            string message = emailIsConfirmed.Succeeded ? Messages.EmailConfirmationSucceeded : Messages.ErrorConfirmingEmail;

            if (!emailIsConfirmed.Succeeded)
            {
                string[] errorsMessages = emailIsConfirmed.Errors.Select(e => e.Description).ToArray();

                _logger.LogError("There was an error when confirming the user {UserId} email {UserEmail}. Error messages: {ErrorMessages}",
                    user.Id, user.Email, String.Join(ErrorConstants.ErrorDelimiter, errorsMessages));
                throw new InvalidArgumentException(Messages.ErrorConfirmingEmail);
            }

            return new EmailConfirmationResponse()
            {
                Message = message
            };
        }
        
        public async Task<PasswordResetEmailResponse> SendPasswordResetEmailAsync(string controllerRouteTemplate, string passwordResetRouteTemplate) 
        {
            User user = await GetUserByUserClaimIdAsync();

            string code = await _userManager.GeneratePasswordResetTokenAsync(user);

            //Encode the code
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            await _emailService.SendPasswordResetEmail(controllerRouteTemplate, passwordResetRouteTemplate, user, code);

            return new PasswordResetEmailResponse()
            {
                Message = Messages.EmailPasswordResetLinkSent
            };
        }

        public async Task<AuthenticationResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            User user = await GetUserByUserClaimIdAsync();

            //Decode the code
            string code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            
            IdentityResult resetPasswordResult = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);

            if (!resetPasswordResult.Succeeded)
            {
                string[] errorsMessages = resetPasswordResult.Errors.Select(e => e.Description).ToArray();

                _logger.LogError("There was an error when resetting the password for user {UserId}. Error messages: {ErrorMessages}",
                    user.Id, String.Join(ErrorConstants.ErrorDelimiter, errorsMessages));
                throw new InvalidArgumentException(Messages.ErrorResetingPassword);
            }

            return new AuthenticationResponse()
            {
                Message = Messages.PasswordResetSussesfuly
            };
        }
        
        public async Task<AuthenticationResponse> LogOutAsync()
        {
            await _signInManager.SignOutAsync();

            await RevokeUserRefreshTokenAsync();

            return new AuthenticationResponse()
            {
                Message = Messages.LogOutSussesfuly
            };
        }

        public async Task<AuthenticationResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            string? refreshToken = _httpContext.Request.Cookies[AuthenticationConstants.RefreshTokenCookieKey];

            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogError("Refresh token missing");
                throw new InvalidArgumentException(Messages.InvalidJwtToken);
            }

            try
            {
                JwtSecurityTokenHandler jwtTokenHandler = new();

                // We set the validate lifetime to false so we can validate the expired access token
                _tokenValidationParameters.ValidateLifetime = false;

                TokenValidationResult tokenVerificationResult = await jwtTokenHandler.ValidateTokenAsync(request.AccessToken, _tokenValidationParameters);

                // Set validate lifetime back to true
                _tokenValidationParameters.ValidateLifetime = true;

                if (!tokenVerificationResult.IsValid)
                {
                    _logger.LogError("Token verification was unsuccessful for token: {Token}. Error: {Error}",
                        request.AccessToken, tokenVerificationResult.Exception.Message);
                    throw new InvalidArgumentException(Messages.InvalidJwtToken);
                }

                SecurityToken validatedToken = tokenVerificationResult.SecurityToken;

                UserRefreshToken? userRefreshToken = await _userRepository.GetUserRefreshTokenWithUserByTokenAsync(refreshToken);

                if (userRefreshToken is null)
                {
                    _logger.LogError("No refresh token exists that matches this token {Token}", request.AccessToken);
                    throw new InvalidArgumentException(Messages.InvalidJwtToken);
                }

                if (userRefreshToken.IsUsed || userRefreshToken.IsRevoked)
                {
                    _logger.LogError("Refresh token {TokenId} has been used or revoked", userRefreshToken.Id);
                    throw new InvalidArgumentException(Messages.InvalidJwtToken);
                }

                string? jti = tokenVerificationResult.Claims
                    .FirstOrDefault(c => c.Key == JwtRegisteredClaimNames.Jti).Value.ToString();

                if (userRefreshToken.JwtId != jti)
                {
                    _logger.LogError("Refresh token {TokenId} has invalid JwtId", userRefreshToken.Id);
                    throw new InvalidArgumentException(Messages.InvalidJwtToken);
                }

                if (userRefreshToken.ExpiryDate <= DateTime.UtcNow)
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
            catch (Exception)
            {
                await RevokeUserRefreshTokenAsync();
                throw;
            }
        }

        private async Task RevokeUserRefreshTokenAsync()
        {
            _httpContext.Response.Cookies.Delete(AuthenticationConstants.RefreshTokenCookieKey);

            User user = await GetUserByUserClaimIdAsync();

            UserRefreshToken? userRefreshToken = await _userRepository.GetUserRefreshTokenByUserIdAsync(user.Id);

            if (userRefreshToken is null)
            {
                _logger.LogError("No refresh token exists that matches this user id {UserId}", user.Id);
                throw new InvalidArgumentException(Messages.InvalidJwtToken);
            }

            userRefreshToken.IsRevoked = true;
            userRefreshToken.IsUsed = true;
            userRefreshToken.ExpiryDate = DateTime.UtcNow;

            _userRepository.UpdateUserRefreshToken(userRefreshToken);
            await _userRepository.SaveChangesAsync();
        }

        private async Task<User> GetUserByUserClaimIdAsync()
        {
            string? userId = _httpContext.User
                .FindFirstValue(AuthenticationConstants.UserIdClaim);

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("User Id claim was not found");
                throw new InvalidArgumentException(Messages.InvalidJwtToken);
            }

            User? user = await _userManager.FindByIdAsync(userId);

            if (user is null || user.IsDeleted)
            {
                _logger.LogError("User with this id {Id} dose not exists or is deleted", userId);
                throw new InvalidArgumentException(Messages.UserDoesNotExist);
            }

            return user;
        }

        private async Task<AuthenticationResponse> GenerateJwtTokenAsync(User user)
        {
            JwtSecurityTokenHandler jwtTokenHandler = new();

            byte[] key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(AuthenticationConstants.UserIdClaim, user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                }),

                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience,
                Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.AccessTokenMinutesLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            SecurityToken token = jwtTokenHandler.CreateToken(tokenDescriptor);
            string accessToken = jwtTokenHandler.WriteToken(token);

            UserRefreshToken? userRefreshToken = await _userRepository.GetUserRefreshTokenByUserIdAsync(user.Id);

            if (userRefreshToken is null)
            {
                userRefreshToken = new()
                {
                    JwtId = token.Id,
                    Token = GenerateRefreshToken(),
                    CreatedDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddMinutes(_jwtConfig.RefreshTokenMinutesLifetime),
                    IsUsed = false,
                    IsRevoked = false,
                    UserId = user.Id
                };

                await _userRepository.AddUserRefreshTokenAsync(userRefreshToken);
            }
            else
            {
                userRefreshToken.JwtId = token.Id;
                userRefreshToken.Token = GenerateRefreshToken();
                userRefreshToken.IsUsed = false;
                userRefreshToken.IsRevoked = false;

                if(userRefreshToken.ExpiryDate <= DateTime.UtcNow)
                {
                    userRefreshToken.CreatedDate = DateTime.UtcNow;
                    userRefreshToken.ExpiryDate = DateTime.UtcNow.AddMinutes(_jwtConfig.RefreshTokenMinutesLifetime);
                }

                _userRepository.UpdateUserRefreshToken(userRefreshToken);
            }

            await _userRepository.SaveChangesAsync();

            _httpContext.Response.Cookies
                .Append(AuthenticationConstants.RefreshTokenCookieKey, userRefreshToken.Token, _cookieOptions);

            return new AuthenticationResponse()
            {
                AccessToken = accessToken
            };
        }

        private string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[_jwtConfig.RefreshTokenLength];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
