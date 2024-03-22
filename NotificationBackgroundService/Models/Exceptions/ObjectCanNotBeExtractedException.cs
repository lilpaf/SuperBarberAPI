namespace NotificationService.Models.Exceptions
{
    public class ObjectCanNotBeExtractedException : Exception
    {
        public ObjectCanNotBeExtractedException(string message) : base(message)
        {
        }
    }
}
