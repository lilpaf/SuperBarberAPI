namespace Business.Models.Responses
{
    public class AuthenticationResponse
    {
        public required string Token { get; init; }
        public required string RefreshToken { get; init; }
    }
}
