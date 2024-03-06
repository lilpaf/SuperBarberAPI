namespace Business.Models.Responses.User
{
    public class AuthenticationResponse
    {
        public string? AccessToken { get; init; }
        public string? Message { get; set; }
    }
}
