using Common.Constants.Resourses;
using Common.Constants;
using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using SuperBarber.Models;
using System.Net;

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

                _logger.LogError("Resource not found for request with id {ID}", context.TraceIdentifier);

                context.Response.StatusCode = response.Error.StatusCode;
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
