using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebApplicationAPI.ViewModel;
using WebApplicationMVC.Models;

namespace WebApplicationMVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public UsersController(ILogger<UsersController> logger, IHttpClientFactory httpClientFactory)
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
        public async Task<IActionResult> Logout()
        {
            try
            {
                var client = CreateClientWithSessionCookie();

                HttpResponseMessage response = await client.PostAsync("User/Logout", null);

                if (response.IsSuccessStatusCode)
                {

                    HttpContext.Session.Clear();

                    return RedirectToAction("Login");

                }
                else
                {
                    return RedirectToAction("Shared", "Error");

                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Shared", "Error");

            }

        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var client = _httpClientFactory.CreateClient("WEB_API");

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(model),
                    System.Text.Encoding.UTF8,
                    "application/json");

                HttpResponseMessage response = await client.PostAsync("User/Login", jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    var responseObject = JsonSerializer.Deserialize<LoginResponse>(jsonResponse);

                    // Lưu vào Session tại phía MVC luôn
                    HttpContext.Session.SetString("UserRole", responseObject.acr);
                    HttpContext.Session.SetInt32("UserID", responseObject.arid);

                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Error: {response.ReasonPhrase}");

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");

                return View(model);
            }
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var client = CreateClientWithSessionCookie();
                HttpResponseMessage response = await client.GetAsync("User/getalluser");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var users = JsonSerializer.Deserialize<List<UserViewModel>>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return View(users);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Không thể lấy danh sách người dùng.");
                    return View(new List<UserViewModel>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user list");
                ModelState.AddModelError(string.Empty, $"Lỗi: {ex.Message}");
                return View(new List<UserViewModel>());
            }
        }

      
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterUser model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var client = _httpClientFactory.CreateClient("WEB_API");

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(model),
                    Encoding.UTF8,
                    "application/json");

                HttpResponseMessage response = await client.PostAsync("User/register", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Đăng ký thành công!";
                    return RedirectToAction("Login", "Users");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Đăng ký thất bại: {errorMessage}");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                ModelState.AddModelError(string.Empty, $"Lỗi: {ex.Message}");
                return View(model);
            }
        }



    }
}

