using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.DTO;
using ClassLibraryDATA.Models;
using Microsoft.AspNetCore.Http;

namespace ClassLibrarySERVICES
{
    public interface IOrderServices
    {
        Task<OrderDTO> CreateOrderAsync(int userId, int foodId, int quantity);
    }
}
