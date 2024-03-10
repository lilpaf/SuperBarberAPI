using Microsoft.AspNetCore.Mvc.Filters;
using Business.Models.Exceptions;

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

                throw new InvalidModelStateException(errorsMessages);
            }

            await next();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //No action needed
        }
    }
}
