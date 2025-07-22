using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;
using ClassLibraryDATA.ViewModels;

namespace ClassLibraryREPO
{
    public interface IOrderRepositories
    {
        Task<List<OrdersView>> GetAllOrdersAsyncByUserId(int userId);
        Task<bool> EditStatus(int id, string orderStatus);
        Task<Order?> GetOrderByIdAsync(int id);
        Task<List<OrdersView>> GetAllBillAsync();
        Task<bool?> CreateOrderAsync(Order order);
        Task<Order?> GetPendingOrderByUserId(int userId);

        Task<Order> UpdateOrderAsync(Order order);
    }
}
