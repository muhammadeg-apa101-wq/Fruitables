using System.ComponentModel.DataAnnotations;

namespace FruitablesFrontToBack.Areas.Admin.ViewModels.ProductVMs
{
    public class CreateProductVM
    {
        [Required(ErrorMessage = "Məhsul adı boş ola bilməz")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Qiymət daxil edin")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Qiymət 0-dan böyük olmalıdır")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Şəkil seçilməlidir")]
        public IFormFile Image { get; set; }

        [Required(ErrorMessage = "Açıqlama boş ola bilməz")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Kateqoriya seçin")]
        [Range(1, int.MaxValue, ErrorMessage = "Kateqoriya seçin")]
        public int CategoryId { get; set; }
    }
}
