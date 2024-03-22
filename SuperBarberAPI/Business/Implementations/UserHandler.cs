using Business.Interfaces;
using Business.Models.Exceptions;
using Common.Constants;
using Common.Constants.Resourses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Persistence.Entities;
using System.Security.Claims;

namespace Business.Implementations
{
    public class UserHandler : IUserHandler
    {
        private readonly ILogger<UserHandler> _logger;
        private readonly UserManager<User> _userManager;
        private static HttpContext _httpContext => new HttpContextAccessor().HttpContext ??
            throw new NotConfiguredException(Messages.NoActiveHttpContext);

        public UserHandler(ILogger<UserHandler> logger, UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<User> GetUserByUserClaimIdAsync()
        {
            string? userId = _httpContext.User
                .FindFirstValue(AuthenticationConstants.UserIdClaimType);

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
    }
}
