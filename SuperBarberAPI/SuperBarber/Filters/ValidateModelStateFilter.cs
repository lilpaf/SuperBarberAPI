using Microsoft.AspNetCore.Mvc.Filters;
using SuperBarber.Models;
using Business.Constants;
using Business.Constants.Messages;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json.Linq;
using Business.Models.Requests.BarberShop;
using Business.Models.Exceptions;
using Org.BouncyCastle.Asn1.X509.Qualified;

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
