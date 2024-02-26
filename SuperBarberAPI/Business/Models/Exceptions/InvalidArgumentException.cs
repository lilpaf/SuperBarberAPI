using Business.Models.Exceptions.General;

namespace Business.Models.Exceptions
{
    public class InvalidArgumentException : ClientSideException
    {
        public InvalidArgumentException(string message) : base(message)
        {
        }
    }
}
