using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;

namespace ClassLibraryREPO
{
    public interface IOrderRepositories
    {
        Task<bool?> CreateOrderAsync(Order order);
        Task<Order?> GetPendingOrderByUserId(int userId);

        Task<Order> UpdateOrderAsync(Order order);
    }
}
