namespace WebApplicationMVC.Models
{
    public class UserViewModel
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string Role { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
    }
}
