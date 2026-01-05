using FruitablesFrontToBack.Areas.Admin.ViewModels.ProductVMs;
using FruitablesFrontToBack.Data;
using FruitablesFrontToBack.Helpers;
using FruitablesFrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            var getAllProductVMs = _context.Products
                .Where(p => !p.IsDeleted)
                .Include(p => p.Category)
                .AsNoTracking()
                .Select(p => new GetAllProductVM
                {
                    Id = p.Id,
                    ImageUrl = p.ImageUrl,
                    Description = p.Description,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category != null ? p.Category.Name : string.Empty
                })
                .ToList();
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

            if (!createProductVM.Image.CheckFileType("image") || !createProductVM.Image.CheckFileSize(2048))
            {
                ModelState.AddModelError("Image", "Yalnız şəkil (jpg, png, webp) və maksimal ölçü 2MB olmalıdır");
                return View(createProductVM);
            }

            var fileName = createProductVM.Image.GenerateFileName();
            var filePath = _env.WebRootPath.GetFilePath("img", fileName);
            createProductVM.Image.SaveFile(filePath);

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

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            var vm = new UpdateProductVM
            {
                Id = product.Id,
                Photo = product.ImageUrl,
                Name = product.Name,
                Price = product.Price,
                CategoryId = product.CategoryId,
                Description = product.Description
            };

            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, UpdateProductVM vm)
        {
            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();

            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            product.Name = vm.Name;
            product.Price = vm.Price;
            product.CategoryId = vm.CategoryId;
            product.Description = vm.Description;

            if (vm.Image != null)
            {
                if (!vm.Image.CheckFileType("image") || !vm.Image.CheckFileSize(2048))
                {
                    ModelState.AddModelError("Image", "Yalnız şəkil (jpg, png, webp) və maksimal ölçü 2MB olmalıdır");
                    return View(vm);
                }

                // delete old image file
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var oldPath = _env.WebRootPath.GetFilePath("img", product.ImageUrl);
                    oldPath.DeleteFile();
                }

                var newFileName = vm.Image.GenerateFileName();
                var newFilePath = _env.WebRootPath.GetFilePath("img", newFileName);
                vm.Image.SaveFile(newFilePath);
                product.ImageUrl = newFileName;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _context.Products.Include(p => p.Category).AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            var vm = new DetailProductVM
            {
                Id = product.Id,
                Photo = product.ImageUrl,
                Name = product.Name,
                Price = product.Price,
                CategoryName = product.Category?.Name,
                Description = product.Description

            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            Product? product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            var filePath = _env.WebRootPath.GetFilePath("img", product.ImageUrl);
            filePath.DeleteFile();
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
