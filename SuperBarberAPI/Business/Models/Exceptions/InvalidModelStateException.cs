using Business.Constants.Messages;
using Business.Models.Exceptions.General;

namespace Business.Models.Exceptions
{
    public class InvalidModelStateException : ClientSideException
    {
        public string[] Errors { get; }

        public InvalidModelStateException(string[] errors) : base(Messages.ModelStateValidationErrorMessage)
        {
            Errors = errors;
        }
    }
}
