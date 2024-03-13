using Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace Business.Models.Requests.Barber
{
    public class RegisterBarberRequest
    {
        [StringLength(DataConstraints.AboutMaxLength)]
        public required string? About { get; init; }
    }
}
