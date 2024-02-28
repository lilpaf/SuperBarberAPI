namespace Business.Configurations
{
    public class EmailConfig
    {
        public required string MailGunUrl { get; init; }
        public required string MailGunDomain { get; init; }
        public required string ApiKey { get; init; }
    }
}
