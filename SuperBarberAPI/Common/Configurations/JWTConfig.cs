namespace Common.Configurations
{
    public class JwtConfig
    {
        public required string Secret { get; init; }
        public required string Issuer { get; init; }
        public required string Audience { get; init; }
        public int AccessTokenMinutesLifetime { get; init; }
        public int RefreshTokenMinutesLifetime { get; init; }
        public int RefreshTokenLength { get; init; }
    }
}
