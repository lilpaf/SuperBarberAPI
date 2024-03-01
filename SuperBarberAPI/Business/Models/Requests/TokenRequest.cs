using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models.Requests
{
    public class TokenRequest
    {
        [Required]
        public string Token { get; init; } = null!;
        
        [Required]
        public string RefreshToken { get; init; } = null!;
    }
}
