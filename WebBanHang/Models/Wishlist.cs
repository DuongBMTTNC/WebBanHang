namespace WebBanHang.Models
{
    public class Wishlist
    {
        public int Id { get; set; }

        public string UserId { get; set; } // Liên kết với User
        public ApplicationUser User { get; set; }

        public int ProductId { get; set; } // Liên kết với Product
        public Product Product { get; set; }
    }
}
