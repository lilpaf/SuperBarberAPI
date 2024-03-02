using Microsoft.AspNetCore.Mvc.Filters;
using SuperBarber.Models;
using Business.Constants;
using Business.Constants.Messages;

namespace SuperBarber.Filters
{
    public class ValidateModelStateFilter : IAsyncActionFilter
    {
        private readonly ILogger<ValidateModelStateFilter> _logger;
        public ValidateModelStateFilter(ILogger<ValidateModelStateFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                string[] errorsMessages = context.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToArray();

                string message = Messages.ModelStateValidationErrorMessage;
                int statusCode = StatusCodes.Status400BadRequest;
                int errorCode = ErrorConstants.ClientSideErrorCode;

                ResponseContent response = new()
                {
                    Error = new ErrorResponse(message, statusCode, errorCode, errorsMessages)
                };

                context.HttpContext.Response.StatusCode = statusCode;
                await context.HttpContext.Response.WriteAsJsonAsync(response);

                _logger.LogError("Invalid model state error with errors: {Errors}", string.Join(ErrorConstants.ErrorDelimiter, errorsMessages));
                return;
            }

            await next();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //No action needed
        }
    }
}
