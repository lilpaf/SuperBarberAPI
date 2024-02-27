using Microsoft.AspNetCore.Mvc.Filters;
using SuperBarber.Models;
using Business.Constants;

namespace SuperBarber.Filters
{
    public class ValidateModelStateFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                string[] errorsMessages = context.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToArray();

                string message = ErrorConstants.ModelStateValidationErrorMessage;
                int statusCode = StatusCodes.Status400BadRequest;
                int errorCode = ErrorConstants.ClientSideErrorCode;

                ResponseContent response = new()
                {
                    Error = new ErrorResponse(message, statusCode, errorCode, errorsMessages)
                };

                context.HttpContext.Response.StatusCode = statusCode;
                await context.HttpContext.Response.WriteAsJsonAsync(response);

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
