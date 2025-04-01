using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanHang.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; }  // Ai nhận thông báo
        public string Message { get; set; } // Nội dung thông báo
        public bool IsRead { get; set; } = false; // Đã đọc hay chưa
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }  // Liên kết với user
    }
}
