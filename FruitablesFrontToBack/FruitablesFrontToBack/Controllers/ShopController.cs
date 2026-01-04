using Microsoft.AspNetCore.Mvc;

namespace FruitablesFrontToBack.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
