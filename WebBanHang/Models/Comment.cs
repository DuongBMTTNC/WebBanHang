using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanHang.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }  // ID của người dùng bình luận
        public string Content { get; set; } // Nội dung bình luận
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Product Product { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
