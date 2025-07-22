using System.Net.Http;
using System.Text.Json;
using Azure;
using ClassLibraryDATA.DTO;
using ClassLibraryDATA.ViewModels;
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
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var client = CreateClientWithSessionCookie();
                var respone = await client.GetAsync("Order/getallbill");
                if (respone.IsSuccessStatusCode)
                {
                    var jsonResponse = await respone.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    List<OrdersView> bills = JsonSerializer.Deserialize<List<OrdersView>>(jsonResponse, options);
                    return View(bills);
                }
                else
                {

                    ModelState.AddModelError(string.Empty, $"Lỗi: {respone.ReasonPhrase}");
                    return View(new List<OrdersView>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách ");
                ModelState.AddModelError(string.Empty, $"Đã xảy ra lỗi: {ex.Message}");
                return View(new List<OrdersView>());

            }
        }

        [HttpPost]
        public async Task<IActionResult> EditStatus(int id, string orderStatus)
        {
            try
            {
                var client = CreateClientWithSessionCookie();
                var response = await client.PostAsync($"Order/EditStatus/{id}/{orderStatus}", null);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Cập nhật trạng thái đơn hàng thành công!";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Cập nhật thất bại: {error}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái đơn hàng");
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ViewOrder()
        {
            try
            {
                var client = CreateClientWithSessionCookie();
                var response = await client.GetAsync("Order/CustomerGetBills");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var bills = JsonSerializer.Deserialize<List<OrdersView>>(jsonResponse, options);

                    return View(bills);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Lỗi: {response.ReasonPhrase}");
                    return View(new List<OrdersView>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách đơn hàng của khách hàng");
                ModelState.AddModelError(string.Empty, $"Đã xảy ra lỗi: {ex.Message}");
                return View(new List<OrdersView>());
            }
        }
    }
}
