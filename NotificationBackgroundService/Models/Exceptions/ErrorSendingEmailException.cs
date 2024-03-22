namespace NotificationService.Models.Exceptions
{
    public class ErrorSendingEmailException : Exception
    {
        public ErrorSendingEmailException(string message) : base(message)
        {
        }
    }
}
