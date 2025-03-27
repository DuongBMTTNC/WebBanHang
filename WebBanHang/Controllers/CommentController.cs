using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Data;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Add(int productId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            var comment = new Comment
            {
                ProductId = productId,
                UserId = User.Identity.Name,
                Content = content,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);

            _context.SaveChanges();

            return RedirectToAction("Details", "Product", new { id = productId });
        }
    }
}
