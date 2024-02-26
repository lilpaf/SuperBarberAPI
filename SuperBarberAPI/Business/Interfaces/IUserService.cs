using Business.Models.Requests;
using Business.Models.Responses;
namespace Business.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticationResponse> RegisterUser(RegisterRequest request);
    }
}
