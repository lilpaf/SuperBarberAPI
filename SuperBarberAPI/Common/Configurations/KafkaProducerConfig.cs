using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Configurations
{
    public class KafkaProducerConfig
    {
        public required string ClientId { get; init; }
        public required string BootstrapServers { get; init; }
    }
}
