using System.ComponentModel.DataAnnotations;

namespace FruitablesFrontToBack.Areas.Admin.ViewModels.CategoryVMs
{
    public class CreateCategoryVM
    {
        [Required(ErrorMessage = "Category Name is required")]
        public string Name { get; set; }
    }
}
