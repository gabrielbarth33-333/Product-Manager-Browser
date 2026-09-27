namespace Domain.Entities
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Code { get; private set; } = string.Empty;
        public string Ean { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public decimal PromotionalPrice { get; private set; }

        private Product() { }

        public Product(Guid id, string name, string code, string ean, decimal price, decimal promotionalPrice)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id n�o pode ser vazio.", nameof(id));

            Id = id;
            SetName(name);
            SetCode(code);
            SetEan(ean);
            SetPrices(price, promotionalPrice);
        }

        public void UpdateDetails(string name, string ean, decimal price, decimal promotionalPrice)
        {
            SetName(name);
            SetEan(ean);
            SetPrices(price, promotionalPrice);
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name n�o pode ser vazio.", nameof(name));

            if (name.Length > 250)
                throw new ArgumentException("Name n�o pode ter mais de 250 caracteres.", nameof(name));

            Name = name;
        }

        private void SetCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code n�o pode ser vazio.", nameof(code));

            if (code.Length > 50)
                throw new ArgumentException("Code n�o pode ter mais de 50 caracteres.", nameof(code));

            Code = code;
        }

        private void SetEan(string ean)
        {
            if (ean.Length > 20)
                throw new ArgumentException("Ean n�o pode ter mais de 20 caracteres.", nameof(ean));

            Ean = ean;
        }

        private void SetPrices(decimal price, decimal promotionalPrice)
        {
            var roundedPrice = Math.Round(price, 2, MidpointRounding.AwayFromZero);
            var roundedPromotional = Math.Round(promotionalPrice, 2, MidpointRounding.AwayFromZero);

            if (roundedPrice <= 0)
                throw new ArgumentException("Price deve ser maior que zero.", nameof(price));

            if (roundedPromotional < 0)
                throw new ArgumentException("PromotionalPrice n�o pode ser negativo.", nameof(promotionalPrice));

            if (roundedPromotional > roundedPrice)
                throw new ArgumentException("PromotionalPrice n�o pode ser maior que Price.", nameof(promotionalPrice));

            Price = roundedPrice;
            PromotionalPrice = roundedPromotional;
        }
    }
}
