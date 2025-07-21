using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibraryREPO
{
    public class UserRepositories : IUserRepositories
    {
        private readonly FoodDeliverContext _context;
        public UserRepositories(FoodDeliverContext context)
        {
            _context = context;
        }

        public async Task<bool> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> ValidateLogin(string email, string password)
        {
            return await _context.Users
          .FirstOrDefaultAsync(a => a.Email == email && a.PasswordHash == password);
        }
    }
}
