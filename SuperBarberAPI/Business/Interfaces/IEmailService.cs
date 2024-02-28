using Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IEmailService
    {
        Task SendConformationEmail(string routeTemplate, string emailConformationAction, User user, string code);
    }
}
