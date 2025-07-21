using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibraryREPO
{
    public class OrderDetailRepositories : IOrderDetailRepositories
    {
        private readonly FoodDeliverContext _context;

        public OrderDetailRepositories(FoodDeliverContext context)
        {
            _context = context;
        }

        public async Task<OrderDetail> CreateOrderDetailAsync(OrderDetail orderDetail)
        {
            orderDetail.TotalPrice = orderDetail.Quantity * orderDetail.Price; // Tính tổng tiền
            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();
            return orderDetail;
        }

        public async Task<OrderDetail?> GetOrderDetail(int orderId, int foodId)
        {
            return await _context.OrderDetails
            .Where(od => od.OrderId == orderId && od.FoodId == foodId)
            .FirstOrDefaultAsync();
        }

        public async Task<decimal?> GetTotalAmountByOrderId(int orderId)
        {
            return await _context.OrderDetails
            .Where(od => od.OrderId == orderId)
            .SumAsync(od => od.TotalPrice);
        }

        public async Task<Order> UpdateOrderAsync(Order existingOrder)
        {
            _context.Orders.Update(existingOrder);
            await _context.SaveChangesAsync();
            return existingOrder;
        }

        public async Task<OrderDetail> UpdateOrderDetailAsync(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
            await _context.SaveChangesAsync();
            return orderDetail;
        }
    }
}
