using Persistence.Entities;

namespace Business.Interfaces
{
    public interface IEmailService
    {
        Task SendConformationEmail(string controllerRouteTemplate, string emailConformationRouteTemplate, User user, string code);
    }
}
