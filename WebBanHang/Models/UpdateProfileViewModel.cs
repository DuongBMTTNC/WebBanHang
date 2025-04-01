using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Models
{
    public class UpdateProfileViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password), Compare("NewPassword", ErrorMessage = "Mật khẩu không khớp!")]
        public string? ConfirmNewPassword { get; set; }
    }
}
