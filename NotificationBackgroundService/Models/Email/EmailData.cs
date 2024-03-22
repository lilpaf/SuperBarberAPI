namespace NotificationService.Models.Email
{
    public class EmailData
    {
        public required string[] RecipientsEmails { get; init; }
        public required string Subject { get; init; }
        public required string Message { get; init; }
    }
}
