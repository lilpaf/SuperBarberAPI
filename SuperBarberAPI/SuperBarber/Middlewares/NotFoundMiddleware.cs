using Common.Constants.Resourses;
using Common.Constants;
using SuperBarber.Models;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;

namespace SuperBarber.Middlewares
{
    public class NotFoundMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<NotFoundMiddleware> _logger;

        public NotFoundMiddleware(
            RequestDelegate next,
            ILogger<NotFoundMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
            {
                string message = Messages.EndpointResourceNotFound;
                int statusCode = (int)HttpStatusCode.NotFound;
                int errorCode = ErrorConstants.ClientSideErrorCode;

                ResponseContent response = new()
                {
                    Error = new(message, statusCode, errorCode)
                };

                string path = context.Request.GetDisplayUrl();

                _logger.LogError("Resource not found for request with request path: {Path}", path);

                context.Response.StatusCode = response.Error.StatusCode;
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
