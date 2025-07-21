using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibraryREPO
{
    public class OrderRepositories : IOrderRepositories
    {
        private readonly FoodDeliverContext _context;
        public OrderRepositories(FoodDeliverContext context)
        {
            _context = context;
        }

        public async Task<bool?> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Order?> GetPendingOrderByUserId(int userId)
        {
            return await _context.Orders
               .Include(o => o.OrderDetails)
               .ThenInclude(od => od.Food)
               .ThenInclude(f => f.Category)
               .Where(o => o.UserId == userId && o.OrderStatus == "Pending")
               .FirstOrDefaultAsync();
        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }
    }
}
