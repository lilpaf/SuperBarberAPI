using Business.Interfaces;
using Business.Models.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Persistence.Entities;
using Common.Constants.Resourses;
using Common.Constants;
using Business.Models.Dtos;

namespace Business.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IKafkaProducer _kafkaProducer;
        private readonly string _scheme;
        private readonly string _host;
        private static HttpContext _httpContext => new HttpContextAccessor().HttpContext ??
           throw new NotConfiguredException(Messages.NoActiveHttpContext);

        public EmailService(ILogger<EmailService> logger, IKafkaProducer kafkaProducer)
        {
            _logger = logger;
            _kafkaProducer = kafkaProducer;
            _scheme = _httpContext.Request.Scheme;
            _host = _httpContext.Request.Host.Value;
            
        }

        public async Task SendConformationEmail(string controllerRouteTemplate, string emailConformationRouteTemplate, User user, string code)
        {
            string callbackUrl = $"{_scheme}://{_host}/{controllerRouteTemplate}/{emailConformationRouteTemplate}?code={code}";

            string message = $"Please confirm your email address <a href=\"{callbackUrl}\">Click here</a>";

            List<string> recipients = new()
            {
                user.Email! // User email will not be null
            };

            await ProduceEmailAsync(recipients, EmailConstants.ConformationEmailSubject, message);
        }

        public async Task SendPasswordResetEmail(string controllerRouteTemplate, string passwordResetRouteTemplate, User user, string code)
        { 
            string callbackUrl = $"{_scheme}://{_host}/{controllerRouteTemplate}/{passwordResetRouteTemplate}?code={code}";

            string message = $"Reset password link <a href=\"{callbackUrl}\">Click here</a>. If you did not request a reset password link, please ignore this email.";

            List<string> recipients = new()
            {
                user.Email! // User email will not be null
            };

            await ProduceEmailAsync(recipients, EmailConstants.ResetPasswordEmailSubject, message);
        }

        private async Task ProduceEmailAsync(IEnumerable<string> recipientsEmails, string subject, string message)
        {
            EmailDataDto emailData = new()
            {
                RecipientsEmails = recipientsEmails,
                Subject = subject,
                Message = message
            };

            await _kafkaProducer.ProduceEmailAsync(emailData);
        }
    }
}
