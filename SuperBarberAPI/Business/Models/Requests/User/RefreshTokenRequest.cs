using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models.Requests.User
{
    public class RefreshTokenRequest
    {
        [Required]
        public required string AccessToken { get; init; }
    }
}
