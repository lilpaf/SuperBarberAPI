namespace Business.Models.Exceptions
{
    public class InvalidModelStateException : Exception
    {
        public string[] Errors { get; }

        public InvalidModelStateException(string message, params string[] errors) : base(message)
        {
            Errors = errors;
        }
    }
}
