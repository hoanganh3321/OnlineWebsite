using ClassLibraryDATA.DTO;
using ClassLibraryDATA.Models;
using ClassLibraryDATA.ViewModels;
using ClassLibrarySERVICES;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplicationAPI.Authorize;

namespace WebApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly IOrderServices _orderServices;

        public OrderController(IOrderServices orderServices)
        {
            _orderServices = orderServices;
        }

        private int GetUserIdFromSession()
        {
            // Lấy UserId từ Session
            var userIdString = HttpContext.Session.GetInt32("UserID")
                ?? throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");


            return userIdString;
        }

        [AttributeRole("Customer")]
        [HttpPost("CustomerOrder/{foodId}/{quantity}")]
        public async Task<ActionResult<OrderDTO>> CustomerOrder(int foodId, int quantity)
        {
            if (foodId <= 0 || quantity <= 0)
            {
                return BadRequest(new { message = "FoodId và Quantity phải lớn hơn 0." });
            }

            try
            {
                int userId = GetUserIdFromSession();
                var createdOrder = await _orderServices.CreateOrderAsync(userId, foodId, quantity);

                return Ok(new { message = "Đặt hàng thành công!", order = createdOrder });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi tạo đơn hàng.", error = ex.Message });
            }
        }

        //https://localhost:7224/api/Order/getallbill
        [AttributeRole("Admin")]
        [HttpGet("getallbill")]
        public async Task<ActionResult<object>> GetAllBills()
        {
            var bills = await _orderServices.GetAllBillAsync();
            if (bills == null)
            {
                return NotFound();
            }
            return Ok(bills);
        }

        //https://localhost:7224/api/Order/EditStatus/{id}/{orderStatus}
        [AttributeRole("Admin")]
        [HttpPost("EditStatus/{id}/{orderStatus}")]
        public async Task<ActionResult> EditStatus(int id, string orderStatus)
        {
            var validStatuses = new[] { "Pending", "Processing", "Completed", "Canceled" };
            if (!validStatuses.Contains(orderStatus))
            {
                ModelState.AddModelError("orderStatus", "Trạng thái không hợp lệ.");
                var order = await _orderServices.GetOrderByIdAsync(id);
                return BadRequest(new OrdersView { Id = id, OrderStatus = order?.OrderStatus });
            }
            var result = await _orderServices.EditStatus(id, orderStatus);
            if (!result) return NotFound();

            return Ok("Cập nhật trạng thái đơn hàng thành công!");
        }
        //https://localhost:7224/api/Order/CustomerGetBills
        [AttributeRole("Customer")]
        [HttpGet("CustomerGetBills")]
        public async Task<ActionResult<IEnumerable<OrdersView>>> GetBillsById()
        {
            int userId = GetUserIdFromSession();
            var ordersList = await _orderServices.GetAllBillByUserId(userId);
            if (ordersList == null)
            {
                return NotFound();
            }
            return Ok(ordersList);
        }
    }
}
