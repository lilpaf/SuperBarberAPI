using Business.Models.Exceptions.General;

namespace Business.Models.Exceptions
{
    public class NotConfiguredException : ServerSideException
    {
        public NotConfiguredException(string message) : base(message) 
        {
        }
    }
}
