using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;
using ClassLibraryDATA.ViewModels;
using ClassLibraryREPO.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ClassLibraryREPO
{
    public class OrderRepositories : GenericRepository<Order>, IOrderRepositories
    {
        public OrderRepositories(FoodDeliverContext context) : base(context)
        {
        }

        public async Task<bool?> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EditStatus(int id, string orderStatus)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;
            order.OrderStatus = orderStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<OrdersView>> GetAllBillAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Food)
                .Select(o => new OrdersView
                {
                    Id = o.OrderId,
                    UserName = o.User.FullName,
                    OrderStatus = o.OrderStatus,
                    PaymentMethod = o.PaymentMethod,
                    CreatedAt = o.CreatedAt,
                    OrderDetails = o.OrderDetails.Select(od => new OrderItemView
                    {
                        FoodName = od.Food.Name,
                        Quantity = od.Quantity,
                        Price = od.Price,
                        TotalPrice = od.TotalPrice
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<List<OrdersView>> GetAllOrdersAsyncByUserId(int userId)
        {
            return await _context.Orders
               .Where(o => o.UserId == userId) 
               .Include(o => o.User)
               .Include(o => o.OrderDetails)
                   .ThenInclude(od => od.Food) 
               .Select(o => new OrdersView
               {
                   OdId= o.OrderId,
                   Id = o.User.UserId,
                   UserName = o.User.FullName,
                   OrderStatus = o.OrderStatus,
                   PaymentMethod = o.PaymentMethod,
                   CreatedAt = o.CreatedAt,
                   OrderDetails = o.OrderDetails.Select(od => new OrderItemView
                   {
                       FoodName = od.Food.Name,
                       Quantity = od.Quantity,
                       Price = od.Price,
                       TotalPrice = od.TotalPrice
                   }).ToList()
               })
               .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
          .Include(o => o.User)
          .Include(o => o.OrderDetails)
          .FirstOrDefaultAsync(o => o.OrderId == id);
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
