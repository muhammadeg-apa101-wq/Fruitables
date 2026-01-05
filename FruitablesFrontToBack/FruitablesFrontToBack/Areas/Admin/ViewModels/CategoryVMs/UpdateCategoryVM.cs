using System.ComponentModel.DataAnnotations;

namespace FruitablesFrontToBack.Areas.Admin.ViewModels.CategoryVMs
{
    public class UpdateCategoryVM
    {
        [Required(ErrorMessage = "Bos ola bilmez!")]
        public string Name { get; set; }
    }
}
