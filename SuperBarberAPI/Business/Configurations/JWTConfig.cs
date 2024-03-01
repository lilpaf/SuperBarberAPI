namespace Business.Configurations
{
    public class JwtConfig
    {
        public required string Secret { get; init; } 
        public required string Issuer { get; init; } 
        public required string Audience { get; init; }
        public int AccessTokenHoursLifetime { get; init; }
        public int RefreshTokenHoursLifetime { get; init; }
        public int RefreshTokenLength { get; init; }
    }
}
