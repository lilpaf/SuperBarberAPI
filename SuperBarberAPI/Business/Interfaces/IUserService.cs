using Business.Models.Requests;
using Business.Models.Responses;
namespace Business.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticationResponse> RegisterUserAsync(UserRegisterRequest request, string controllerRouteTemplate, string emailConformationRouteTemplate);
        
        Task<AuthenticationResponse> LoginUserAsync(UserLoginRequest request);

        Task<EmailConfirmationResponse> ConfirmEmailAsync(EmailConfirmationRequest request);
    }
}
