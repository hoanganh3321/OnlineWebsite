using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.DTO;
using ClassLibraryDATA.Models;

namespace ClassLibraryREPO
{
    public interface IMessageRepository
    {
        Task<IEnumerable<CustomerChatDto>> GetAllCustomersChattedAsync();
        Task AddMessageAsync(Message message);
        Task<IEnumerable<Message>> GetMessagesAsync(int user1, int user2);
    }
}
