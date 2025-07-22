using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.DTO;
using ClassLibraryDATA.Models;
using ClassLibraryDATA.ViewModels;
using Microsoft.AspNetCore.Http;

namespace ClassLibrarySERVICES
{
    public interface IOrderServices
    {
        Task<List<OrdersView>> GetAllBillByUserId(int userId);
        Task<bool> EditStatus(int id, string orderStatus);
        Task<Order?> GetOrderByIdAsync(int id);
        Task<List<OrdersView>> GetAllBillAsync();
        Task<OrderDTO> CreateOrderAsync(int userId, int foodId, int quantity);
    }
}
