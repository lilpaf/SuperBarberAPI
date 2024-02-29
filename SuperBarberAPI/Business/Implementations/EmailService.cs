using Business.Configurations;
using Business.Constants.Messages;
using Business.Interfaces;
using Business.Models.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Persistence.Entities;
using System.Net.Mail;
using System.Text;
using System.Net;

namespace Business.Implementations
{
    public class EmailService : IEmailService
    {
        private const string ConformationEmailSubject = "Email Verification";
        private readonly ILogger<EmailService> _logger;
        private readonly SmtpConfig _smtpConfig;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailService(
            ILogger<EmailService> logger,
            IHttpContextAccessor httpContextAccessor,
            IOptions<SmtpConfig> smtpConfig)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _smtpConfig = smtpConfig.Value;
        }

        public async Task SendConformationEmail(string controllerRouteTemplate, string emailConformationRouteTemplate, User user, string code)
        {
            string scheme = _httpContextAccessor.HttpContext!.Request.Scheme;
            string host = _httpContextAccessor.HttpContext.Request.Host.Value;

            string callbackUrl = $"{scheme}://{host}/{controllerRouteTemplate}/{emailConformationRouteTemplate}?userId={user.Id}&code={code}";

            string message = $"Please confirm your email address <a href=\"{callbackUrl}\">Click here</a>";

            List<string> recipients = new ()
            {
                user.Email! // User email will not be null
            };

            await SendEmail(recipients, ConformationEmailSubject, message);
        }

        private async Task SendEmail(IList<string> recipients, string subject, string message)
        {
            try
            {
                MailMessage mailMessage = new()
                {
                    From = new MailAddress(_smtpConfig.SmtpUsername),
                    Subject = subject,
                    Body = message,
                    SubjectEncoding = Encoding.UTF8,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(string.Join(", ", recipients));

                using SmtpClient client = new(_smtpConfig.SmtpServer, _smtpConfig.SmtpPort)
                {
                    Credentials = new NetworkCredential(_smtpConfig.SmtpUsername, _smtpConfig.SmtpPassword),
                    EnableSsl = _smtpConfig.EnableSSL
                };

                await client.SendMailAsync(mailMessage);

                _logger.LogInformation("Successfully sent email to {recipient}", mailMessage.To);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Exception occurred when sending the email: {Message}", exception.Message);

                throw new ErrorSendingEmailException(Messages.ErrorSendingEmail);
            }
        }
    }
}
