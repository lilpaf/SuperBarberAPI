using Business.Models.Exceptions;
using Business.Models.Exceptions.General;
using Common.Constants;
using Common.Constants.Resourses;
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
            catch (InvalidModelStateException exception)
            {
                _logger.LogError("Invalid model state error with errors: {Errors}", 
                    string.Join(ErrorConstants.ErrorDelimiter, exception.Errors));

                ResponseContent response = new()
                {
                    Error = new(exception.Message, exception.StatusCode, exception.ErrorCode, exception.Errors)
                };

                context.Response.StatusCode = response.Error.StatusCode;
                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

                string message = Messages.GeneralErrorMessage;
                int statusCode = StatusCodes.Status500InternalServerError;
                int errorCode = ErrorConstants.ServerSideErrorCode;

                if (exception is ClientSideException clientException)
                {
                    message = clientException.Message;
                    statusCode = clientException.StatusCode;
                    errorCode = clientException.ErrorCode;
                }
                else if (exception is ServerSideException serverException)
                {
                    message = serverException.Message;
                    statusCode = serverException.StatusCode;
                    errorCode = serverException.ErrorCode;
                }

                ResponseContent response = new()
                {
                    Error = new(message, statusCode, errorCode)
                };

                context.Response.StatusCode = response.Error.StatusCode;
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
