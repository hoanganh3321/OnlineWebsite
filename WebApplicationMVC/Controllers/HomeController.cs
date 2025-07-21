using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using ClassLibraryDATA.DTO;
using Microsoft.AspNetCore.Mvc;
using WebApplicationMVC.Models;

namespace WebApplicationMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
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

    public async Task<IActionResult> Index()
    {
        try
        {
            var client = CreateClientWithSessionCookie();
            HttpResponseMessage response = await client.GetAsync("Food/list");
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                List<FoodDTO> foods = JsonSerializer.Deserialize<List<FoodDTO>>(jsonResponse, options);
                return View(foods);
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"Lỗi: {response.ReasonPhrase}");
                return View(new List<FoodDTO>());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách ");
            ModelState.AddModelError(string.Empty, $"Đã xảy ra lỗi: {ex.Message}");
            return View(new List<FoodDTO>());
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    //AI TEST
    [HttpPost]
    public async Task<IActionResult> SuggestFood(string userInput)
    {
        var client = _httpClientFactory.CreateClient("WEB_API");

        // ✅ Sửa tại đây: gửi object thay vì chuỗi
        var requestBody = new { userInput = userInput };
        var content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync("ai/suggest", content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Không thể lấy gợi ý từ AI");
            return View("Index", new List<FoodDTO>());
        }

        var keywordJson = await response.Content.ReadAsStringAsync();
        var keywords = JsonSerializer.Deserialize<List<string>>(keywordJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


        List<FoodDTO> foodList = new();
        if (keywords.Any())
        {
            var queryString = string.Join("&", keywords.Select(k => $"keyword={Uri.EscapeDataString(k)}"));
            var foodResponse = await client.GetAsync($"food/search-by-keywords?{queryString}");

            if (foodResponse.IsSuccessStatusCode)
            {
                var foodJson = await foodResponse.Content.ReadAsStringAsync();
                foodList = JsonSerializer.Deserialize<List<FoodDTO>>(foodJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<FoodDTO>();
            }
        }

        // ✅ 3) Truyền dữ liệu sang View
        ViewBag.UserInput = userInput;
        ViewBag.AIKeywords = keywords;

        return View("Index", foodList);
    }



}
