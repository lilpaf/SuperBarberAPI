namespace Business.Models.Email
{
    public class EmailData
    {
        public required IEnumerable<string> RecipientsEmails { get; init; }
        public required string Subject { get; init; }
        public required string Message { get; init; }
    }
}
