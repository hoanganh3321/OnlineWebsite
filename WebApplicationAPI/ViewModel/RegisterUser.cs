using System.ComponentModel.DataAnnotations;

namespace WebApplicationAPI.ViewModel
{
    public class RegisterUser
    {
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string PasswordHash { get; set; }
        public string? PhoneNumber { get; set; }

        public string address { get; set; }

        public string role { get; set; } = "Customer";

        public DateTime createat { get; set; } = DateTime.Now;
    }
}
