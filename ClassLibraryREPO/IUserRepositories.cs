using ClassLibraryDATA.Models;

namespace ClassLibraryREPO
{
    public interface IUserRepositories
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> AddUserAsync(User user);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> ValidateLogin(string email, string password);
    }
}