using Confluent.Kafka;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using NotificationService.Constants;
using NotificationService.Models.Configurations;
using NotificationService.Models.Email;
using NotificationService.Models.Exceptions;
using NotificationService.Resourses;
using System.Text.Json;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace NotificationService
{
    public class EmailService : BackgroundService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly KafkaConsumerConfig _kafkaConfig;
        private readonly KafkaEmailConsumerConfig _kafkaEmailConfig;
        private readonly ConsumerConfig _config;
        private readonly SmtpConfig _smtpConfig;

        public EmailService(
            ILogger<EmailService> logger,
            IOptions<KafkaEmailConsumerConfig> kafkaEmailConfig,
            IOptions<SmtpConfig> smtpConfig,
            IOptions<KafkaConsumerConfig> kafkaConfig)
        {
            _logger = logger;
            _kafkaConfig = kafkaConfig.Value;
            _kafkaEmailConfig = kafkaEmailConfig.Value;
            _smtpConfig = smtpConfig.Value;
            _config = new ConsumerConfig
            {
                BootstrapServers = _kafkaConfig.BootstrapServers,
                GroupId = _kafkaConfig.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var consumer = new ConsumerBuilder<Ignore, string>(_config).Build();

            consumer.Subscribe(_kafkaEmailConfig.Topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<Ignore, string> message = consumer.Consume(stoppingToken);

                _logger.LogInformation("Received email message to send with email data {EmailData}", message.Message.Value);

                EmailData? emailData = JsonSerializer.Deserialize<EmailData>(message.Message.Value);

                if (emailData is null)
                {
                    _logger.LogError("There was an error deserializing the value");
                    throw new ObjectCanNotBeExtractedException(Messages.ErrorDeserializingValue);
                }

                await SendEmailAsync(emailData.RecipientsEmails, emailData.Subject, emailData.Message);
            }
        }

        private async Task SendEmailAsync(IEnumerable<string> recipientsEmails, string subject, string message)
        {
            try
            {
                InternetAddressList recipients = new();

                foreach (var email in recipientsEmails)
                {
                    recipients.Add(MailboxAddress.Parse(email));
                }

                MimeMessage mailMessage = new();
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
