using System.Net.Http;
using System.Text;
using System.Text.Json;
using ClassLibraryDATA.Models;
using Microsoft.AspNetCore.Mvc;
using WebApplicationAPI.ViewModel;

namespace WebApplicationMVC.Controllers
{
    public class ProductReviewsController : Controller
    {
        private readonly ILogger<ProductReviewsController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductReviewsController(ILogger<ProductReviewsController> logger, IHttpClientFactory httpClientFactory)
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

        [HttpGet]
        public async Task<IActionResult> Index(int foodId)
        {
            var client = CreateClientWithSessionCookie();

            var response = await client.GetAsync($"ProductReview/{foodId}");
            if (!response.IsSuccessStatusCode)
                return View(new List<ProductReviewViewModel>());

            var json = await response.Content.ReadAsStringAsync();
            var reviews = JsonSerializer.Deserialize<List<ProductReviewViewModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            ViewBag.ProductId = foodId;
            return View(reviews);
        }


        [HttpPost]
        public async Task<IActionResult> AddReview(ProductReviewViewModel model)
        {

            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                TempData["Error"] = "Bạn phải đăng nhập để đánh giá!";
                return RedirectToAction("Index", new { productId = model.ProductId });
            }

            model.CustomerId = userId.Value;
            var client = CreateClientWithSessionCookie();
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("ProductReview", content);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Gửi đánh giá thất bại!";
                return RedirectToAction("Index", new { productId = model.ProductId });
            }

            TempData["Success"] = "Đánh giá thành công!";
            return RedirectToAction("Index", new { productId = model.ProductId });
        }
    }
}
