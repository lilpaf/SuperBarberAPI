using Business.Constants;
using Microsoft.AspNetCore.Http;

namespace Business.Models.Exceptions.General
{
    public class ClientSideException : Exception
    {
        public int StatusCode { get; }
        public int ErrorCode { get; }

        public ClientSideException(string message) : base(message)
        {
            StatusCode = StatusCodes.Status400BadRequest;
            ErrorCode = ErrorConstants.ClientSideErrorCode;
        }
    }
}
