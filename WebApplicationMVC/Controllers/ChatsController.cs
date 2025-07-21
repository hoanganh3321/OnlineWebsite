using ClassLibraryDATA.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace WebApplicationMVC.Controllers
{
    public class ChatsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ChatsController> _logger;


        public ChatsController(IHttpClientFactory httpClientFactory, ILogger<ChatsController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Message> messages = new List<Message>();

            try
            {
                var client = _httpClientFactory.CreateClient("WEB_API");
                var response = await client.GetAsync("chat/history");

                if (response.IsSuccessStatusCode)
                {
                    messages = await response.Content.ReadFromJsonAsync<IEnumerable<Message>>() ?? new List<Message>();
                }
                else
                {
                    _logger.LogError("API trả về lỗi: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi fetch lịch sử chat");
            }

            return View(messages);
        }
    }
}
