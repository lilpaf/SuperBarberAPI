using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Configurations
{
    public class KafkaEmailProducerConfig
    {
        public required string Topic { get; init; }
    }
}
