using FruitablesFrontToBack.Areas.Admin.ViewModels.ProductVMs;
using FruitablesFrontToBack.Data;
using FruitablesFrontToBack.Helpers;
using FruitablesFrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FruitablesFrontToBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context;

        public ProductController(IWebHostEnvironment env, AppDbContext context)
        {
            _env = env;
            _context = context;
        }

        public IActionResult Index()
        {
            IEnumerable<GetAllProductVM> getAllProductVMs = _context.Products
    .Where(p => !p.IsDeleted)
    .Select(p => new GetAllProductVM
    {
        Id = p.Id,
        ImageUrl = p.ImageUrl,
        Description = p.Description,
        Name = p.Name,
        Price = p.Price,
        CategoryName = p.Category.Name
    }).ToList();
            return View(getAllProductVMs);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories
        .Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        })
        .ToList();

            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateProductVM createProductVM)
        {
            ViewBag.Categories = _context.Categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToList();


            if (!ModelState.IsValid)
            {
                return View(createProductVM);
            }


            if (createProductVM.Image == null)
            {
                ModelState.AddModelError("Image", "Şəkil seçilməlidir");
                return View(createProductVM);
            }


            if (createProductVM.Image.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("Image", "Şəklin ölçüsü 2 MB-dan böyük ola bilməz");
                return View(createProductVM);
            }


            string[] allowedTypes = { "image/jpeg", "image/png", "image/webp" };

            if (!allowedTypes.Contains(createProductVM.Image.ContentType))
            {
                ModelState.AddModelError("Image", "Yalnız şəkil faylları (jpg, png, webp) qəbul olunur");
                return View(createProductVM);
            }


            string folder = Path.Combine(_env.WebRootPath, "img");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid() + "_" + createProductVM.Image.FileName;
            string filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                createProductVM.Image.CopyTo(stream);
            }


            var product = new Product
            {
                Name = createProductVM.Name,
                Price = createProductVM.Price,
                Description = createProductVM.Description,
                ImageUrl = fileName,
                CategoryId = createProductVM.CategoryId
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            Product? product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            string folder = Path.Combine(_env.WebRootPath, "img");
            string filePath = Path.Combine(folder, product.ImageUrl);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }



}
