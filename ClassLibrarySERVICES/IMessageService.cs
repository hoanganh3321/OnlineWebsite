using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;

namespace ClassLibrarySERVICES
{
    public interface IMessageService
    {
        Task<IEnumerable<int>> GetAllCustomersChattedAsync();
        Task SaveMessageAsync(Message message);
        Task<IEnumerable<Message>> GetChatHistoryAsync(int user1, int user2);
    }
}
