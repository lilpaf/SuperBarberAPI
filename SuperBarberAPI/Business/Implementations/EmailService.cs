using Business.Configurations;
using Business.Constants.Messages;
using Business.Interfaces;
using Business.Models.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Persistence.Entities;
using MimeKit;
using MimeKit.Text;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using Business.Constants;

namespace Business.Implementations
{
    public class EmailService : IEmailService
    {
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

            List<string> recipients = new()
            {
                user.Email!, // User email will not be null
            };

            await SendEmail(recipients, EmailConstants.ConformationEmailSubject, message);
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
