using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;
using ClassLibraryREPO;

namespace ClassLibrarySERVICES
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepositories userRepositories;

        public UserServices(IUserRepositories userRepositories)
        {
            this.userRepositories = userRepositories;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await userRepositories.GetAllUsersAsync();
        }

        public async Task<bool> RegisterUserAsync(User user)
        {
            var validateEmail = await userRepositories.GetUserByEmailAsync(user.Email);
            if (validateEmail == null)
            {
                return await userRepositories.AddUserAsync(user);
            }
            else
            {
                return false;
            }
        }

        public async Task<User> ValidateLoginAsync(string email, string password)
        {
            var user = await userRepositories.ValidateLogin(email, password);
            return user;
        }
    }
}
