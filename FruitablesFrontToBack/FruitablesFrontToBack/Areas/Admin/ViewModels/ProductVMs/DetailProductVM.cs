namespace FruitablesFrontToBack.Areas.Admin.ViewModels.ProductVMs
{
    public class DetailProductVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? Photo { get; set; }
        public string CategoryName { get; set; }

        public int CategoryId { get; set; }
    }
}
