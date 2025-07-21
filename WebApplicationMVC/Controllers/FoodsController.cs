using System.Text.Json;
using ClassLibraryDATA.DTO;
using ClassLibraryDATA.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationMVC.Controllers
{
    public class FoodsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<FoodsController> _logger;

        public FoodsController(IHttpClientFactory httpClientFactory, ILogger<FoodsController> logger)
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

        public async Task<IActionResult> Detail([FromQuery(Name = "foodId")] int? foodId)
        {
            try
            {
                if (!foodId.HasValue || foodId <= 0)
                {
                    _logger.LogWarning("Invalid or missing foodId: {FoodId}", foodId);
                    ModelState.AddModelError("", "ID món ăn không hợp lệ hoặc không được cung cấp.");
                    return View(null);
                }

                var client = CreateClientWithSessionCookie();
                HttpResponseMessage response = await client.GetAsync($"Food/detail/{foodId}");
                _logger.LogInformation("API response status: {StatusCode}", response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("API response JSON: {Json}", json);
                    var food = JsonSerializer.Deserialize<FoodDTO>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (food == null)
                    {
                        _logger.LogError("Failed to deserialize JSON to FoodDTO");
                        ModelState.AddModelError("", "Không thể giải mã dữ liệu món ăn.");
                        return View(null);
                    }

                    return View(food);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("API returned non-success status: {StatusCode}, Content: {Content}", response.StatusCode, errorContent);
                    ModelState.AddModelError("", $"Không tìm thấy món ăn: {response.ReasonPhrase}");
                    return View(null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching food detail for ID {FoodId}", foodId);
                ModelState.AddModelError("", $"Đã xảy ra lỗi: {ex.Message}");
                return View(null);
            }
        }
    }
}
