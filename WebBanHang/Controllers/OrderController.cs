using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Data;
using WebBanHang.Models;
using WebBanHang.Services;

namespace WebBanHang.Controllers
{
    public class OrderController : Controller
    {
        private readonly NotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public OrderController(NotificationService notificationService, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _notificationService = notificationService;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> ConfirmOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound();

            order.Status = "Confirmed"; // Cập nhật trạng thái đơn hàng
            await _context.SaveChangesAsync();

            // Gửi thông báo cho Customer
            await _notificationService.AddNotificationAsync(order.UserId, "Đơn hàng của bạn đã được xác nhận!");

            // Gửi thông báo cho Staff
            var staffUsers = await _userManager.GetUsersInRoleAsync("Staff");
            foreach (var staff in staffUsers)
            {
                await _notificationService.AddNotificationAsync(staff.Id, "Có đơn hàng mới được đặt!");
            }

            return RedirectToAction("OrderList");
        }
    }
}
