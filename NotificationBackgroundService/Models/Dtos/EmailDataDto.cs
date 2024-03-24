namespace NotificationService.Models.Dtos
{
    public class EmailDataDto
    {
        public required string[] RecipientsEmails { get; init; }
        public required string Subject { get; init; }
        public required string Message { get; init; }
    }
}
