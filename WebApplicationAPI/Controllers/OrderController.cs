using ClassLibraryDATA.DTO;
using ClassLibraryDATA.Models;
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


    }
}
