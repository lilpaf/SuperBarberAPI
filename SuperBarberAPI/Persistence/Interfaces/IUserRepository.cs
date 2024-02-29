namespace Persistence.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> UserIsActiveAndExistsByEmailAsync(string email);
    }
}
