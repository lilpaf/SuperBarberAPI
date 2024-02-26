using Business.Models.Exceptions.General;

namespace Business.Models.Exceptions
{
    public class InvalidValidationObjectException : ClientSideException
    {
        public InvalidValidationObjectException(string message) : base(message)
        {
        }
    }
}
