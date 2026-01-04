using FruitablesFrontToBack.Data;
using FruitablesFrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FruitablesFrontToBack.Controllers
{
    public class ShopDetailController : Controller
    {
        private readonly AppDbContext _context;

        public ShopDetailController(AppDbContext context)
        {
            _context = context;
        }

        // Accept an id and return the product with its Category
        public async Task<IActionResult> Index(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id.Value);

            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}
