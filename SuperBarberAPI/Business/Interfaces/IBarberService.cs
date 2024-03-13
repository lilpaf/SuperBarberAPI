using Business.Models.Requests.Barber;
using Business.Models.Responses.Barber;

namespace Business.Interfaces
{
    public interface IBarberService
    {
        Task<RegisterBarberResponse> RegisterBarberAsync(RegisterBarberRequest request);
    }
}
