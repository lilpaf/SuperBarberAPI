using Business.Models.Exceptions.General;

namespace Business.Models.Exceptions
{
    public class InvalidModelStateException : ClientSideException
    {
        public string[] Errors { get; }

        public InvalidModelStateException(string message, params string[] errors) : base(message)
        {
            Errors = errors;
        }
    }
}
