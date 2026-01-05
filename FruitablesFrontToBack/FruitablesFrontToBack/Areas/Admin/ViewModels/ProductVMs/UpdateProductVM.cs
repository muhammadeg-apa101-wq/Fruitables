using System.ComponentModel.DataAnnotations;

namespace FruitablesFrontToBack.Areas.Admin.ViewModels.ProductVMs
{
    public class UpdateProductVM
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }

        public IFormFile? Image { get; set; }

        public int CategoryId { get; set; }

        public string? Photo { get; set; }
    }
}
