using System.Text.Json.Serialization;

namespace SuperBarber.Models
{
    public class ErrorResponse
    {
        public string ErrorMessage { get; }
        public int StatusCode { get; }
        public int ErrorCode { get; }
        public string[]? ErrorsMessages { get; }

        public ErrorResponse(string errorMessage, int statusCode, int errorCode, string[]? errorsMessages = null)
        {
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
            ErrorCode = errorCode;
            ErrorsMessages = errorsMessages;
        }
    }
}
