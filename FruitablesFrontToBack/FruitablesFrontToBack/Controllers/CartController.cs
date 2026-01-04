using FruitablesFrontToBack.Data;
using FruitablesFrontToBack.Models;
using FruitablesFrontToBack.ViewModels.CartVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FruitablesFrontToBack.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private const string SessionCartKey = "Cart.Items";

        public CartController(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartVM>>(SessionCartKey) ?? new List<CartVM>();

            var details = new List<CartDetailVM>();

            foreach (var c in cart)
            {
                var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == c.Id);
                if (product is null) continue;

                details.Add(new CartDetailVM
                {
                    Id = product.Id,
                    Image = product.ImageUrl,
                    Name = product.Name,
                    Category = product.Category?.Name ?? string.Empty,
                    Price = product.Price,
                    Count = c.Count,
                    TotalPrice = product.Price * c.Count
                });
            }

            return View(details);
        }


        [HttpGet]
        public async Task<JsonResult> Add(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null)
                return Json(new { success = false, message = "Product not found" });

            var cart = HttpContext.Session.GetObject<List<CartVM>>(SessionCartKey) ?? new List<CartVM>();

            var existing = cart.FirstOrDefault(x => x.Id == id);
            if (existing is null)
            {
                cart.Add(new CartVM { Id = id, Count = 1, Price = product.Price });
            }
            else
            {
                existing.Count++;
            }

            HttpContext.Session.SetObject(SessionCartKey, cart);

            var totalCount = cart.Sum(x => x.Count);
            return Json(new { success = true, count = totalCount });
        }


        [HttpGet]
        public JsonResult Count()
        {
            var cart = HttpContext.Session.GetObject<List<CartVM>>(SessionCartKey) ?? new List<CartVM>();
            var totalCount = cart.Sum(x => x.Count);
            return Json(new { count = totalCount });
        }


        [HttpPost]
        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartVM>>(SessionCartKey) ?? new List<CartVM>();
            var item = cart.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.SetObject(SessionCartKey, cart);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
