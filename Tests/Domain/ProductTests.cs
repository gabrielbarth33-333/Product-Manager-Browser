using Domain.Entities;
using FluentAssertions;

namespace Tests.Domain
{
    public class ProductTests
    {
        [Fact]
        public void Constructor_Should_Create_Product_With_Valid_Data()
        {
            var product = new Product(Guid.NewGuid(), "Product Name", "CODE001", "7891234567890", 100m, 80m);

            product.Name.Should().Be("Product Name");
            product.Code.Should().Be("CODE001");
            product.Ean.Should().Be("7891234567890");
            product.Price.Should().Be(100m);
            product.PromotionalPrice.Should().Be(80m);
        }

        [Fact]
        public void Constructor_Should_Round_Prices_To_Two_Decimals()
        {
            var product = new Product(Guid.NewGuid(), "Product Name", "CODE001", "", 100.999m, 80.999m);

            product.Price.Should().Be(101.00m);
            product.PromotionalPrice.Should().Be(81.00m);
        }

        [Fact]
        public void Constructor_Should_Throw_When_Id_Is_Empty()
        {
            Action act = () => new Product(Guid.Empty, "Product Name", "CODE001", "", 100m, 80m);

            act.Should().Throw<ArgumentException>().WithMessage("*Id*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_Should_Throw_When_Name_Is_Empty(string? name)
        {
            Action act = () => new Product(Guid.NewGuid(), name!, "CODE001", "", 100m, 80m);

            act.Should().Throw<ArgumentException>().WithMessage("*Name*");
        }

        [Fact]
        public void Constructor_Should_Throw_When_Name_Exceeds_Max_Length()
        {
            var longName = new string('a', 251);

            Action act = () => new Product(Guid.NewGuid(), longName, "CODE001", "", 100m, 80m);

            act.Should().Throw<ArgumentException>().WithMessage("*Name*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_Should_Throw_When_Code_Is_Empty(string? code)
        {
            Action act = () => new Product(Guid.NewGuid(), "Product Name", code!, "", 100m, 80m);

            act.Should().Throw<ArgumentException>().WithMessage("*Code*");
        }

        [Fact]
        public void Constructor_Should_Throw_When_Code_Exceeds_Max_Length()
        {
            var longCode = new string('a', 51);

            Action act = () => new Product(Guid.NewGuid(), "Product Name", longCode, "", 100m, 80m);

            act.Should().Throw<ArgumentException>().WithMessage("*Code*");
        }

        [Fact]
        public void Constructor_Should_Throw_When_Ean_Exceeds_Max_Length()
        {
            var longEan = new string('a', 21);

            Action act = () => new Product(Guid.NewGuid(), "Product Name", "CODE001", longEan, 100m, 80m);

            act.Should().Throw<ArgumentException>().WithMessage("*Ean*");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_Should_Throw_When_Price_Is_Not_Positive(decimal price)
        {
            Action act = () => new Product(Guid.NewGuid(), "Product Name", "CODE001", "", price, 0m);

            act.Should().Throw<ArgumentException>().WithMessage("*Price*");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-0.01)]
        public void Constructor_Should_Throw_When_PromotionalPrice_Is_Negative(decimal promotionalPrice)
        {
            Action act = () => new Product(Guid.NewGuid(), "Product Name", "CODE001", "", 100m, promotionalPrice);

            act.Should().Throw<ArgumentException>().WithMessage("*PromotionalPrice*");
        }

        [Fact]
        public void Constructor_Should_Throw_When_PromotionalPrice_Greater_Than_Price()
        {
            Action act = () => new Product(Guid.NewGuid(), "Product Name", "CODE001", "", 100m, 101m);

            act.Should().Throw<ArgumentException>().WithMessage("*PromotionalPrice*");
        }

        [Fact]
        public void UpdateDetails_Should_Update_Allowed_Fields()
        {
            var product = new Product(Guid.NewGuid(), "Product Name", "CODE001", "", 100m, 80m);

            product.UpdateDetails("Updated Name", "1234567890123", 200m, 150m);

            product.Name.Should().Be("Updated Name");
            product.Ean.Should().Be("1234567890123");
            product.Price.Should().Be(200m);
            product.PromotionalPrice.Should().Be(150m);
        }

        [Fact]
        public void UpdateDetails_Should_Throw_When_Name_Is_Empty()
        {
            var product = new Product(Guid.NewGuid(), "Product Name", "CODE001", "", 100m, 80m);

            Action act = () => product.UpdateDetails("", "", 100m, 80m);

            act.Should().Throw<ArgumentException>().WithMessage("*Name*");
        }

        [Fact]
        public void UpdateDetails_Should_Throw_When_Price_Is_Invalid()
        {
            var product = new Product(Guid.NewGuid(), "Product Name", "CODE001", "", 100m, 80m);

            Action act = () => product.UpdateDetails("Product Name", "", 0m, 80m);

            act.Should().Throw<ArgumentException>().WithMessage("*Price*");
        }

        [Fact]
        public void UpdateDetails_Should_Throw_When_PromotionalPrice_Greater_Than_Price()
        {
            var product = new Product(Guid.NewGuid(), "Product Name", "CODE001", "", 100m, 80m);

            Action act = () => product.UpdateDetails("Product Name", "", 100m, 101m);

            act.Should().Throw<ArgumentException>().WithMessage("*PromotionalPrice*");
        }
    }
}
