namespace WebBanHang.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }  // Liên kết với tài khoản người dùng
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
