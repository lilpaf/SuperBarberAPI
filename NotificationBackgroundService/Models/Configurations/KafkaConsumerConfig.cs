using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Models.Configurations
{
    public class KafkaConsumerConfig
    {
        public required string GroupId { get; init; }
        public required string BootstrapServers { get; init; }
    }
}
