using ClassLibrarySERVICES;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplicationAPI.Authorize;

namespace WebApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public ChatController(IMessageService messageService)
        {
            _messageService = messageService;
        }
        private int GetUserIdFromSession()
        {
        
            var userIdString = HttpContext.Session.GetInt32("UserID")
                ?? throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");


            return userIdString;
        }
        [AttributeRole("Customer")]
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            int senderId = GetUserIdFromSession();
            var messages = await _messageService.GetChatHistoryAsync(senderId, 3);
            return Ok(messages);
        }

       
        [AttributeRole("Admin")]
        [HttpGet("history-admin")]
        public async Task<IActionResult> GetHistoryAdmin([FromQuery] int customerId)
        {
            int adminId = GetUserIdFromSession(); // Lấy AdminId từ Session
            var messages = await _messageService.GetChatHistoryAsync(adminId, customerId);
            return Ok(messages);
        }

        
        [AttributeRole("Admin")]
        [HttpGet("customers")]
        public async Task<IActionResult> GetAllCustomersChatted()
        {
            var customers = await _messageService.GetAllCustomersChattedAsync();
            return Ok(customers);
        }
    }
}
