namespace Application.Commands.Products
{
    public class CreateProductCommand
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Ean { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal PromotionalPrice { get; set; }
    }
}

