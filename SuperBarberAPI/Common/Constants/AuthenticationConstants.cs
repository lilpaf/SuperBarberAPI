using System.Security.Claims;

namespace Common.Constants
{
    public class AuthenticationConstants
    {
        public const string RefreshTokenCookieKey = "refreshToken";
        public const string AccessTokenCookieKey = "accessToken";
        public const string UserIdClaimType = ClaimTypes.NameIdentifier;
    }
}
