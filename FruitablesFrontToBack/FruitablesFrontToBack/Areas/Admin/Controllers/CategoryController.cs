using FruitablesFrontToBack.Areas.Admin.ViewModels.CategoryVMs;
using FruitablesFrontToBack.Data;
using FruitablesFrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;

namespace FruitablesFrontToBack.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class CategoryController : Controller
    {

        public readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> categories = await _context.Categories
                    .OrderByDescending(m => m.Id)
                    .Where(m => !m.IsDeleted)
                    .ToListAsync();

            IEnumerable<GetAllCategoryVM> getAllCategoryVMs = categories.Select(m => new GetAllCategoryVM
            {
                Id = m.Id,
                Name = m.Name,
            });

            return View(getAllCategoryVMs);
        }

      public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryVM createCategoryVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool isExist = await _context.Categories
                .AnyAsync(m => m.Name.ToLower().Trim() == createCategoryVM.Name.ToLower().Trim());
            if (isExist)
            {
                ModelState.AddModelError("Name", "This category name already exist");
                return View();
            }
            Category category = new Category
            {
                Name = createCategoryVM.Name,
            };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            Category? category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (category == null) return NotFound();

            UpdateCategoryVM updateCategoryVM = new()
            {
                Name = category.Name
            };

            return View(updateCategoryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, UpdateCategoryVM updateCategoryVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Category? category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (category == null) return NotFound();
            bool isExist = await _context.Categories
                .AnyAsync(m => m.Name.ToLower().Trim() == updateCategoryVM.Name.ToLower().Trim() && m.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This category name already exist");
                return View();
            }
            category.Name = updateCategoryVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            Category? category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (category == null) return NotFound();
            category.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            Category? category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (category == null) return NotFound();
            DetailCategoryVM detailCategoryVM = new()
            {
                Id = category.Id,
                Name = category.Name
            };
            return View(detailCategoryVM);
        }
    }

    
        
    
}
