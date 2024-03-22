using Business.Interfaces;
using Business.Models.Email;
using Business.Models.Exceptions;
using Common.Configurations;
using Common.Constants.Resourses;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;

namespace Business.Implementations
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly ILogger<KafkaProducer> _logger;
        private readonly IProducer<Null, string> _producer;
        private readonly KafkaEmailProducerConfig _kafkaEmailProducerConfig;

        public KafkaProducer(
            ILogger<KafkaProducer> logger,
            IProducer<Null, string> producer,
            IOptions<KafkaEmailProducerConfig> kafkaEmailProducerConfig)
        {
            _logger = logger;
            _producer = producer;
            _kafkaEmailProducerConfig = kafkaEmailProducerConfig.Value;
        }

        public async Task ProduceEmailAsync(EmailData emailData)
        {
            try
            {
                string message = JsonSerializer.Serialize(emailData);

                await _producer.ProduceAsync(_kafkaEmailProducerConfig.Topic, new Message<Null, string> { Value = message });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Exception occurred when producing the email to Kafka: {Message}", exception.Message);

                throw new ErrorSendingEmailException(Messages.ErrorSendingEmail);
            }
        }

    }
}
