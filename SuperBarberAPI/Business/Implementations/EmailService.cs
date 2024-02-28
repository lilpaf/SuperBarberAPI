using Azure.Core;
using Business.Configurations;
using Business.Constants.Messages;
using Business.Interfaces;
using Business.Models.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Persistence.Entities;
using Persistence.Interfaces;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Business.Implementations
{
    public class EmailService : IEmailService
    {
        private const string ConformationEmailSubject = "Email Verification";
        private readonly ILogger<EmailService> _logger;
        private readonly EmailConfig _emailConfig;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public EmailService(
            ILogger<EmailService> logger,
            IHttpContextAccessor httpContextAccessor,
            IOptions<EmailConfig> emailConfig)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _emailConfig = emailConfig.Value;
        }

        public async Task SendConformationEmail(string routeTemplate, string emailConformationAction, User user, string code)
        {
            string scheme = _httpContextAccessor.HttpContext.Request.Scheme;
            string host = _httpContextAccessor.HttpContext.Request.Host.Value;

            string callbackUrl = HtmlEncoder.Default.Encode($"{scheme}://{host}/{routeTemplate}/{emailConformationAction}?userId={user.Id}&code={code}");

            string emailBody = $"Please confirm your email address <a href=\"{callbackUrl}\">CLick here</a>";

            await SendEmail(user.Email!, ConformationEmailSubject, emailBody);
        }

        private async Task SendEmail(string recipient, string subject, string body)
        {
            RestClientOptions options = new RestClientOptions(_emailConfig.MailGunUrl)
            {
                Authenticator = new HttpBasicAuthenticator("api", _emailConfig.ApiKey)
            };

            RestClient client = new RestClient(options);

            RestRequest request = new RestRequest(string.Empty, Method.Post);

            request.AddParameter("domain", _emailConfig.MailGunDomain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Super Barber Mailgun Sandbox"); //<postmaster@sandbox6140dc4e97064290a6e64916a405da20.mailgun.org>
            request.AddParameter("to", recipient);
            request.AddParameter("subject", subject);
            request.AddParameter("text", body);

            RestResponse response = await client.ExecuteAsync(request);
            
            if(!response.IsSuccessful)
            {
                _logger.LogError("Error sending email to {recipient}. Error message: {message}", recipient, response.ErrorMessage);
                throw new ErrorSendingEmailException(Messages.ErrorSendingConformationEmail);
            }

            _logger.LogInformation("Successfully sent email to {recipient}", recipient);
        }
    }
}
