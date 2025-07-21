using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibraryREPO
{
    public class MessageRepository : IMessageRepository
    {
        private readonly FoodDeliverContext _context;

        public MessageRepository(FoodDeliverContext context)
        {
            _context = context;
        }

        public async Task AddMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<int>> GetAllCustomersChattedAsync()
        {
            return await _context.Messages
                 .Where(m => m.ReceiverId == 3)
                 .Select(m => m.SenderId)
                 .Distinct()
                 .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(int user1, int user2)
        {
            return await _context.Messages
            .Where(m => (m.SenderId == user1 && m.ReceiverId == user2) ||
                        (m.SenderId == user2 && m.ReceiverId == user1))
            .OrderBy(m => m.SentAt)
            .ToListAsync();
        }
    }
}
