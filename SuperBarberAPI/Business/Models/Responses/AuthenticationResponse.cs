namespace Business.Models.Responses
{
    public class AuthenticationResponse
    {
        public string? AccessToken { get; init; }
        public string? Message { get; set; }
    }
}
