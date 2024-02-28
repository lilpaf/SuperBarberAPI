using Business.Models.Exceptions.General;

namespace Business.Models.Exceptions
{
    public class ErrorSendingEmailException : ServerSideException
    {
        public ErrorSendingEmailException(string message) : base(message) 
        {
        }
    }
}
