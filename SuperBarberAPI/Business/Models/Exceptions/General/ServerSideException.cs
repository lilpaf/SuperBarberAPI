using Microsoft.AspNetCore.Http;

namespace Business.Models.Exceptions.General
{
    public class ServerSideException : Exception
    {
        public int StatusCode { get; }
        public int ErrorCode { get; }

        public ServerSideException(string message) : base(message)
        {
            StatusCode = StatusCodes.Status400BadRequest;
            ErrorCode = 1;
        }
    }
}
