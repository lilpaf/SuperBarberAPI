using Business.Models.Requests;
namespace Business.Interfaces
{
    public interface IUserService
    {
        Task RegisterUser(RegisterRequest request);
    }
}
