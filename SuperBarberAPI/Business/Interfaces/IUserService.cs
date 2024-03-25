using Business.Models.Requests.User;
using Business.Models.Responses.User;
namespace Business.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticationResponse> RegisterUserAsync(UserRegisterRequest request, string controllerRouteTemplate, string emailConformationRouteTemplate);
        
        Task<AuthenticationResponse> LoginUserAsync(UserLoginRequest request);

        Task<EmailConfirmationResponse> ConfirmEmailAsync(EmailConfirmationRequest request);

        Task RefreshTokenAsync();

        Task<PasswordResetEmailResponse> SendPasswordResetEmailAsync(ResetPasswordEmailRequest request, string controllerRouteTemplate, string passwordResetRouteTemplate);

        Task<AuthenticationResponse> ResetPasswordAsync(ResetPasswordRequest request);

        Task<AuthenticationResponse> LogOutAsync();
    }
}
