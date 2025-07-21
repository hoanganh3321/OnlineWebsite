using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;

namespace ClassLibraryREPO
{
    public interface IOrderDetailRepositories
    {
        Task<OrderDetail> CreateOrderDetailAsync(OrderDetail orderDetail);
        Task<OrderDetail> UpdateOrderDetailAsync(OrderDetail orderDetail);
        Task<OrderDetail?> GetOrderDetail(int orderId, int foodId);
        Task<decimal?> GetTotalAmountByOrderId(int orderId);
        Task<Order> UpdateOrderAsync(Order existingOrder);
    }
}
