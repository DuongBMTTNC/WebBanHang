using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProductController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        [HttpGet]
        public IActionResult Index(string searchString, decimal? minPrice, decimal? maxPrice)
        {
            var products = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString));
            }

            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            // Lưu lại giá trị tìm kiếm vào ViewData
            ViewData["CurrentFilter"] = searchString;
            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;

            return View(products.ToList());
        }

        // Chi tiết sản phẩm
        public async Task<IActionResult> Details(int? id)
        {
            var product = await _context.Products
        .Include(p => p.Ratings)
            .ThenInclude(r => r.User)
        .Include(p => p.Comments)
            .ThenInclude(c => c.User)
        .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Tạo sản phẩm
        [Authorize(Roles = "Admin, Staff")]
        public IActionResult Create()
        {
  
            return View();
        }
        // POST: Tạo sản phẩm

        [Authorize(Roles = "Admin, Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Định nghĩa đường dẫn thư mục lưu ảnh
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    // Tạo thư mục nếu chưa tồn tại
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Tạo tên file ảnh duy nhất
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploadPath, fileName);

                    // Lưu ảnh vào thư mục
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    // Lưu đường dẫn ảnh vào database
                    product.ImageUrl = "/images/" + fileName;
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Chỉnh sửa sản phẩm
        [Authorize(Roles = "Admin, Staff")]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [Authorize(Roles = "Admin, Staff")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products.FindAsync(id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin sản phẩm
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;

                    // Nếu có ảnh mới, thì thay ảnh cũ
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Xóa ảnh cũ (nếu có)
                        if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                        {
                            string oldFilePath = Path.Combine("wwwroot", existingProduct.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Lưu ảnh mới
                        string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                        string extension = Path.GetExtension(imageFile.FileName);
                        string uniqueFileName = fileName + "_" + Guid.NewGuid().ToString() + extension;
                        string filePath = Path.Combine("wwwroot/images", uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        // Cập nhật đường dẫn ảnh mới vào database
                        existingProduct.ImageUrl = "/images/" + uniqueFileName;
                    }

                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra, vui lòng thử lại.");
                }
            }
            return View(product);
        }


        // GET: Xóa sản phẩm
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        // POST: Xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            // Xóa ảnh trong thư mục wwwroot nếu có
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.ImageUrl.TrimStart('/'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult AddComment()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddComment(int productId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("Nội dung bình luận không được để trống.");
            }

            // Lấy User ID của tài khoản đang đăng nhập
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("Bạn cần đăng nhập để bình luận.");
            }

            // Tạo bình luận mới
            var comment = new Comment
            {
                UserId = user.Id, // Gán UserId là của tài khoản đang đăng nhập
                ProductId = productId,
                Content = content,
                CreatedAt = DateTime.UtcNow // Tự động cập nhật ngày bình luận
            };

            // Lưu vào database
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok("Bình luận đã được thêm.");
        }

        public async Task<IActionResult> Wishlist()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var wishlistItems = await _context.Wishlists
                .Where(w => w.UserId == user.Id)
                .Include(w => w.Product)
                .ToListAsync();

            return View(wishlistItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("Bạn cần đăng nhập để thêm vào yêu thích.");
            }

            var existingWishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == user.Id && w.ProductId == productId);

            if (existingWishlist == null)
            {
                var wishlist = new Wishlist
                {
                    UserId = user.Id,
                    ProductId = productId
                };

                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Sản phẩm đã được thêm vào danh sách yêu thích." });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("Bạn cần đăng nhập để xóa khỏi danh sách yêu thích.");
            }

            var wishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == user.Id && w.ProductId == productId);

            if (wishlistItem != null)
            {
                _context.Wishlists.Remove(wishlistItem);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Sản phẩm đã được xóa khỏi danh sách yêu thích." });
        }

        [HttpPost]
        public async Task<IActionResult> AddRating(int productId, int stars)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var rating = new Rating
            {
                ProductId = productId,
                Stars = stars,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            // Cập nhật số sao trung bình
            var product = await _context.Products
                .Include(p => p.Ratings)
                .FirstOrDefaultAsync(p => p.Id == productId);

            var averageRating = product.Ratings.Any() ? product.Ratings.Average(r => r.Stars) : 0;

            return Json(new
            {
                averageRating = averageRating.ToString("0.0"),
                user = User.Identity.Name,
                stars = stars,
                createdAt = rating.CreatedAt.ToString("yyyy-MM-dd HH:mm")
            });
        }
    }
}
