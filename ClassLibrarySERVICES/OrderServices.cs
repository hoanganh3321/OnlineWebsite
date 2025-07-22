using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.DTO;
using ClassLibraryDATA.Models;
using ClassLibraryDATA.ViewModels;
using ClassLibraryREPO;
using Microsoft.AspNetCore.Http;

namespace ClassLibrarySERVICES
{
    public class OrderServices : IOrderServices
    {

        private readonly IOrderRepositories _repo;
        private readonly IFoodRepositories _foodrepo;
        private readonly IOrderDetailRepositories _Odrepo;

        public OrderServices(IOrderRepositories repo, IFoodRepositories foodrepo, IOrderDetailRepositories odrepo)
        {
            _repo = repo;
            _foodrepo = foodrepo;
            _Odrepo = odrepo;
        }

        public async Task<OrderDTO> CreateOrderAsync(int userId, int foodId, int quantity)
        {
            var existingOrder = await GetOrCreatePendingOrder(userId);
            var food = await _foodrepo.GetByIdAsync(foodId)
                ?? throw new InvalidOperationException("Món ăn không tồn tại.");

            await AddOrUpdateOrderDetail(existingOrder.OrderId, foodId, quantity, food.Price);

            existingOrder.TotalAmount = await _Odrepo.GetTotalAmountByOrderId(existingOrder.OrderId) ?? 0;
            await _Odrepo.UpdateOrderAsync(existingOrder);

       
            var orderDetails = existingOrder.OrderDetails.Select(od => new OrderDetailDTO
            {
                Quantity = od.Quantity,
                TotalPrice = od.TotalPrice,
                Food = new FoodDTO
                {
                    FoodId = od.Food.FoodId,
                    Name = od.Food.Name,
                    Description = od.Food.Description,
                    Price = od.Food.Price,
                    ImageUrl = od.Food.ImageUrl,
                    CategoryId = od.Food.CategoryId,
                    IsAvailable = od.Food.IsAvailable,
                    CreatedAt = od.Food.CreatedAt,
                    Category = od.Food.Category == null ? null : new CategoryDTO
                    {
                        CategoryId = od.Food.Category.CategoryId,
                        CategoryName = od.Food.Category.CategoryName
                    }
                }
            }).ToList();

            return new OrderDTO
            {
                OrderId = existingOrder.OrderId,
                TotalAmount = existingOrder.TotalAmount,
                CreatedAt = existingOrder.CreatedAt,
                OrderDetails = orderDetails
            };
        }

        // Lấy đơn hàng Pending của User hoặc tạo mới nếu chưa có
        private async Task<Order> GetOrCreatePendingOrder(int userId)
        {
            var existingOrder = await _repo.GetPendingOrderByUserId(userId);
            if (existingOrder != null)
            {
                existingOrder.CreatedAt = DateTime.UtcNow;
                return await _repo.UpdateOrderAsync(existingOrder);
            }

         
            var newOrder = new Order
            {

                UserId = userId,
                CreatedAt = DateTime.UtcNow,

            };
           
            var isCreated = await _repo.CreateOrderAsync(newOrder);

            
            if (isCreated != true) // Kiểm tra `null` hoặc `false`
            {
                throw new InvalidOperationException("Không thể tạo đơn hàng mới.");
            }

            return newOrder;
        }

        //    Thêm hoặc cập nhật OrderDetail (giỏ hàng)
        private async Task AddOrUpdateOrderDetail(int orderId, int foodId, int quantity, decimal price)
        {
            // Kiểm tra xem sản phẩm này đã có trong giỏ hàng chưa
            var existingDetail = await _Odrepo.GetOrderDetail(orderId, foodId);

            if (existingDetail != null)
            {
                // Nếu sản phẩm đã có, chỉ cập nhật số lượng và tổng tiền
                existingDetail.Quantity += quantity;
                existingDetail.TotalPrice = existingDetail.Price * existingDetail.Quantity;
                await _Odrepo.UpdateOrderDetailAsync(existingDetail);
            }
            else
            {
                // Nếu chưa có, tạo một OrderDetail mới
                var newDetail = new OrderDetail
                {
                    OrderId = orderId,
                    FoodId = foodId,
                    Quantity = quantity,
                    Price = price,
                    TotalPrice = price * quantity
                };
                await _Odrepo.CreateOrderDetailAsync(newDetail);
            }
        }

        public async Task<List<OrdersView>> GetAllBillAsync()
        {
            return await _repo.GetAllBillAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _repo.GetOrderByIdAsync(id);
        }

        public async Task<bool> EditStatus(int id, string orderStatus)
        {
            return await _repo.EditStatus(id, orderStatus);
        }

        public async Task<List<OrdersView>> GetAllBillByUserId(int userId)
        {
            return await _repo.GetAllOrdersAsyncByUserId(userId);
        }
    }
}
