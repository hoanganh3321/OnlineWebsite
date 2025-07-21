using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationMVC.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public OrdersController(ILogger<OrdersController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
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
        [HttpPost]
        public async Task<IActionResult> Order(int foodId, int quantity)
        {
            try
            {
               
                var client = CreateClientWithSessionCookie();

                
                var response = await client.PostAsync(
                    $"Order/CustomerOrder/{foodId}/{quantity}", null);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Đặt món thành công!";
                    return RedirectToAction("Index", "Home");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    TempData["ErrorMessage"] = "Bạn cần đăng nhập trước khi đặt món.";
                    return RedirectToAction("Login", "Users");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Lỗi khi đặt món: {error}";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

    }
}
