namespace Business.Models.Responses
{
    public class AuthenticationResponse
    {
        public required string AccessToken { get; init; }
        //public required DateTime AccessTokenExpiryDate { get; init; }
        public required string RefreshToken { get; init; }
    }
}
