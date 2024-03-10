namespace Common.Configurations
{
    public class SmtpConfig
    {
        public required string SmtpUsername { get; init; }
        public required string SmtpPassword { get; init; }
        public required string SmtpServer { get; init; }
        public int SmtpPort { get; init; }
        public bool EnableSSL { get; init; }
    }
}
