using Persistence.Entities;

namespace Business.Interfaces
{
    public interface IUserHandler
    {
        Task<User> GetUserByUserClaimIdAsync();
    }
}
