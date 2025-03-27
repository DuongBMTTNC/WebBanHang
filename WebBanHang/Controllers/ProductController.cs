using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Danh sách sản phẩm
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        // Chi tiết sản phẩm
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
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

        // GET: Tạo sản phẩm
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
  
            return View();
        }
        // POST: Tạo sản phẩm
       
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

        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            return View(product);
        }

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


    }
}
