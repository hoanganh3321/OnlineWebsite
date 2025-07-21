using ClassLibraryDATA.Models;

namespace ClassLibrarySERVICES
{
    public interface IUserServices
    {
        Task<bool> RegisterUserAsync(User user);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> ValidateLoginAsync(string email, string password);
    }
}