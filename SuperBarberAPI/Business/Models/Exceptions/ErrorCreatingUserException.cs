using Business.Models.Exceptions.General;

namespace Business.Models.Exceptions
{
    public class ErrorCreatingUserException : ServerSideException
    {
        public ErrorCreatingUserException(string message) : base(message) 
        { 
        }
    }
}
