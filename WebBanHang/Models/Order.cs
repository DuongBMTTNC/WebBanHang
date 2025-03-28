namespace WebBanHang.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }  // Liên kết với tài khoản người dùng
        public ApplicationUser User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Chờ xác nhận";  // Trạng thái đơn hàng

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public decimal TotalPrice => OrderItems.Sum(i => i.Total);
    }
}
