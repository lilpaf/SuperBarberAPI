using Business.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IKafkaProducer
    {
        Task ProduceEmailAsync(EmailData emailData);
    }
}
