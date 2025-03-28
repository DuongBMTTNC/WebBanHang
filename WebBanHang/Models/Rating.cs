namespace WebBanHang.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int Stars { get; set; } // Số sao (1-5)
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Product? Product { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
