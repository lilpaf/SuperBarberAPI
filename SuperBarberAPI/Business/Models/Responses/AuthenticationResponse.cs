using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models.Responses
{
    public class AuthenticationResponse
    {
        public string Token { get; set; }

        public bool Result { get; set; }

        //ToDo may be deleted
        public List<string> Errors { get; set; }
    }
}
