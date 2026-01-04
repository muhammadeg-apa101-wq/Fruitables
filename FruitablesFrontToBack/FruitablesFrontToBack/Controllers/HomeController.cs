using FruitablesFrontToBack.Data;
using FruitablesFrontToBack.Models;
using FruitablesFrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq; 

namespace FruitablesFrontToBack.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var productsQuery = _context.Products.Include(p => p.Category).AsQueryable();

            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            var products = await productsQuery.ToListAsync();
            var categories = await _context.Categories.ToListAsync();

            HomeVM homeVM = new()
            {
                Products = products,
                Categories = categories,
                SelectedCategoryId = categoryId
            };

            return View(homeVM);
        }
    }
}
