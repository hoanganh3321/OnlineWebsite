using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;

namespace ClassLibraryREPO
{
    public interface IMessageRepository
    {
        Task<IEnumerable<int>> GetAllCustomersChattedAsync();
        Task AddMessageAsync(Message message);
        Task<IEnumerable<Message>> GetMessagesAsync(int user1, int user2);
    }
}
