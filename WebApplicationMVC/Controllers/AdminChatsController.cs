using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationMVC.Controllers
{
    public class AdminChatsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AdminChatsController> _logger;

        public AdminChatsController(IHttpClientFactory httpClientFactory, ILogger<AdminChatsController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        private HttpClient CreateClientWithSessionCookie()
        {
            var client = _httpClientFactory.CreateClient("WEB_API");
            var sessionCookie = HttpContext.Request.Cookies[".AspNetCore.Session"];
            if (!string.IsNullOrEmpty(sessionCookie))
            {
                client.DefaultRequestHeaders.Add("Cookie", $".AspNetCore.Session={sessionCookie}");
            }
            return client;
        }
        public IActionResult Index()
        {
            return View();
        }
        // API gọi sang WebApplicationAPI để lấy lịch sử chat
        [HttpGet]
        public async Task<IActionResult> GetHistory(int customerId)
        {
            try
            {
                var client = CreateClientWithSessionCookie(); 
                var response = await client.GetAsync($"chat/history-admin?customerId={customerId}");
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch sử chat admin");
                return StatusCode(500, "Lỗi server khi lấy lịch sử chat.");
            }
        }
        
        
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var client = CreateClientWithSessionCookie();
                var response = await client.GetAsync("chat/customers");
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách khách hàng");
                return StatusCode(500, "Lỗi server khi lấy danh sách khách hàng.");
            }
        }


    }
}
