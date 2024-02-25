namespace Business.Models.Exceptions
{
    public class InvalidValidationObjectException : Exception
    {
        public InvalidValidationObjectException(string message) : base(message)
        {
        }
    }
}
