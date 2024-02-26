namespace SuperBarber.Models
{
    public class ErrorResponse
    {
        public string ErrorMessage { get; }
        public int StatusCode { get; }
        public int ErrorCode { get; }

        public ErrorResponse(string errorMessage, int statusCode, int errorCode)
        {
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}
