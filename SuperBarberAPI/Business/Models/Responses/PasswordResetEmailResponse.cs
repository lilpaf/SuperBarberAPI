using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models.Responses
{
    public class PasswordResetEmailResponse
    {
        public required string Message { get; init; }
    }
}
