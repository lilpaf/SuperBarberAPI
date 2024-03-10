using Business.Interfaces;
using Business.Models.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Persistence.Entities;
using MimeKit;
using MimeKit.Text;
using Common.Configurations;
using Common.Constants.Messages;
using Common.Constants;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Business.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly SmtpConfig _smtpConfig;
        private static HttpContext _httpContext => new HttpContextAccessor().HttpContext ??
           throw new NotConfiguredException(Messages.NoActiveHttpContext);

        public EmailService(
            ILogger<EmailService> logger,
            IOptions<SmtpConfig> smtpConfig)
        {
            _logger = logger;
            _smtpConfig = smtpConfig.Value;
        }

        public async Task SendConformationEmail(string controllerRouteTemplate, string emailConformationRouteTemplate, User user, string code)
        {
            string scheme = _httpContext.Request.Scheme;
            string host = _httpContext.Request.Host.Value;

            string callbackUrl = $"{scheme}://{host}/{controllerRouteTemplate}/{emailConformationRouteTemplate}?code={code}";

            string message = $"Please confirm your email address <a href=\"{callbackUrl}\">Click here</a>";

            List<string> recipients = new()
            {
                user.Email! // User email will not be null
            };

            await SendEmail(recipients, EmailConstants.ConformationEmailSubject, message);
        }
        
        public async Task SendPasswordResetEmail(string controllerRouteTemplate, string passwordResetRouteTemplate, User user, string code)
        {
            string scheme = _httpContext.Request.Scheme; // ToDo will be changed
            string host = _httpContext.Request.Host.Value; // ToDo will be changed

            string callbackUrl = $"{scheme}://{host}/{controllerRouteTemplate}/{passwordResetRouteTemplate}?code={code}";

            string message = $"Reset password link <a href=\"{callbackUrl}\">Click here</a>. If you did not request a reset password link, please ignore this email.";

            List<string> recipients = new()
            {
                user.Email! // User email will not be null
            };

            await SendEmail(recipients, EmailConstants.ResetPasswordEmailSubject, message);
        }

        private async Task SendEmail(IList<string> recipientsEmails, string subject, string message)
        {
            try
            {
                InternetAddressList recipients = new ();

                foreach (var email in recipientsEmails)
                {
                    recipients.Add(MailboxAddress.Parse(email));
                }

                MimeMessage mailMessage = new ();
                mailMessage.From.Add(MailboxAddress.Parse(_smtpConfig.SmtpUsername));
                mailMessage.To.AddRange(recipients);
                mailMessage.Subject = subject;
                mailMessage.Body = new TextPart(TextFormat.Html) { Text = message };

                using SmtpClient client = new();

                await client.ConnectAsync(_smtpConfig.SmtpServer, _smtpConfig.SmtpPort);
                await client.AuthenticateAsync(_smtpConfig.SmtpUsername, _smtpConfig.SmtpPassword);

                await client.SendAsync(mailMessage);
                
                _logger.LogInformation("Successfully sent email to {Recipients}", string.Join(EmailConstants.RecipientsDelimiter, mailMessage.To));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Exception occurred when sending the email: {Message}", exception.Message);

                throw new ErrorSendingEmailException(Messages.ErrorSendingEmail);
            }
        }
    }
}
