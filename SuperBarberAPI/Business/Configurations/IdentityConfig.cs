namespace Business.Configurations
{
    public class IdentityConfig
    {
        public int DataProtectionTokenHoursLifetime { get; init; }
        public int LockoutMinutesTimeSpan { get; init; }
        public int MaxFailedAccessAttempts { get; init; }
    }
}
