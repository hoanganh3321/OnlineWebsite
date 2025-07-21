 using ClassLibraryDATA.Models;
using ClassLibrarySERVICES;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplicationAPI.Authorize;
using WebApplicationAPI.ViewModel;

namespace WebApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }


        //https://localhost:7224/api/User/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var account = await _userServices.ValidateLoginAsync(model.Email, model.Password);

            if (account == null)
            {
                return Unauthorized(new { message = "Invalid email or password.1" });
            }

            HttpContext.Session.SetString("UserRole", account.Role);
            HttpContext.Session.SetInt32("UserID", account.UserId);

            return Ok(new
            {
                message = "Login successful",
                acr = account.Role,
                arid = account.UserId
            });
        }

        // https://localhost:7224/api/User/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Logout successful" });
        }


        // https://localhost:7224/api/User/getalluser
        [HttpGet("getalluser")]
        [AttributeRole("Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUser()
        {
            var users = await _userServices.GetAllUsersAsync();
            return Ok(users);
        }


        // https://localhost:7224/api/User/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterNewUser([FromBody] RegisterUser customer)
        {
            if (!ModelState.IsValid) return BadRequest();
            var user = new User
            {
                FullName = customer.FullName,
                Email = customer.Email,
                PasswordHash = customer.PasswordHash,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.address,
                Role = customer.role,
                CreatedAt = customer.createat
            };
            bool success = await _userServices.RegisterUserAsync(user);
            if (!success)
            {            
                return BadRequest("Đăng ký thất bại kiểm tra lại email có thể bị trùng");
            }
            return Ok(user);    
        }
    }
}
