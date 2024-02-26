using Business.Constants;
using Business.Models.Exceptions.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SuperBarber.Models;

namespace SuperBarber.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

                string message = exception.Message;
                int statusCode = StatusCodes.Status500InternalServerError;
                int errorCode = ErrorConstants.ServerSideErrorCode;

                if (exception is ClientSideException clientException)
                {
                    statusCode = clientException.StatusCode;
                    errorCode = clientException.ErrorCode;
                }
                else if (exception is ServerSideException serverException)
                {
                    statusCode = serverException.StatusCode;
                    errorCode = serverException.ErrorCode;
                }

                ResponseContent response = new()
                {
                    Error = new(exception.Message, statusCode, errorCode)
                };

                context.Response.StatusCode = response.Error.StatusCode;

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
