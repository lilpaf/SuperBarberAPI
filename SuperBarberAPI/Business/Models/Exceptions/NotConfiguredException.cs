namespace Business.Models.Exceptions
{
    public class NotConfiguredException : Exception
    {
        public NotConfiguredException(string message) : base(message) 
        {
        }
    }
}
