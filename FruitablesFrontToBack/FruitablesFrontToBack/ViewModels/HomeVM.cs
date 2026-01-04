using FruitablesFrontToBack.Models;

namespace FruitablesFrontToBack.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }

        public int? SelectedCategoryId { get; set; }
    }
}
