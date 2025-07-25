using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.DTO;
using ClassLibraryDATA.Models;
using ClassLibraryREPO;

namespace ClassLibrarySERVICES
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repo;

        public MessageService(IMessageRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<CustomerChatDto>> GetAllCustomersChattedAsync()
        {
            return await _repo.GetAllCustomersChattedAsync();
        }

        public async Task<IEnumerable<Message>> GetChatHistoryAsync(int user1, int user2)
        {
            return await _repo.GetMessagesAsync(user1, user2);
        }

        public async Task SaveMessageAsync(Message message)
        {
            await _repo.AddMessageAsync(message);
        }
    }
}
