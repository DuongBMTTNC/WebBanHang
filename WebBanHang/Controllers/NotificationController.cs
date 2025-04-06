using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Services;
using WebBanHang.Models;
using Microsoft.AspNetCore.Authorization;
using WebBanHang.Data;
using Microsoft.EntityFrameworkCore;
namespace WebBanHang.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationController(NotificationService notificationService, UserManager<ApplicationUser> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> StaffNotifications()
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == null)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(notifications);
        }
    }
}
