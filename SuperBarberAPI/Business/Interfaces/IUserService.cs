using Business.Models.Requests;
using Business.Models.Responses;
namespace Business.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticationResponse> RegisterUserAsync(UserRegisterRequest request, string controllerRouteTemplate, string emailConformationRouteTemplate);
        
        Task<AuthenticationResponse> LoginUserAsync(UserLoginRequest request);

        Task<EmailConfirmationResponse> ConfirmEmailAsync(EmailConfirmationRequest request);

        Task<AuthenticationResponse> RefreshTokenAsync(RefreshTokenRequest request);

        Task<PasswordResetEmailResponse> SendPasswordResetEmailAsync(string controllerRouteTemplate, string passwordResetRouteTemplate);

        Task<AuthenticationResponse> ResetPasswordAsync(ResetPasswordRequest request);

        Task<AuthenticationResponse> LogOutAsync();
    }
}
