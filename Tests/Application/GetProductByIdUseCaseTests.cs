using Application.Interfaces;
using Application.UseCases.Products;
using Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace Tests.Application
{
    public class GetProductByIdUseCaseTests
    {
        private readonly IProductRepository _productRepository;
        private readonly GetProductByIdUseCase _useCase;

        public GetProductByIdUseCaseTests()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _useCase = new GetProductByIdUseCase(_productRepository);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Return_Product_When_It_Exists()
        {
            var productId = Guid.NewGuid();
            var product = new Product(productId, "Product Name", "CODE001", "7891234567890", 100m, 80m);

            _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>())
                .Returns(product);

            var result = await _useCase.ExecuteAsync(productId);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Id.Should().Be(productId);
            result.Value.Name.Should().Be("Product Name");
            result.Value.Code.Should().Be("CODE001");
        }

        [Fact]
        public async Task ExecuteAsync_Should_Return_Failure_When_Product_Not_Found()
        {
            var productId = Guid.NewGuid();

            _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>())
                .Returns((Product?)null);

            var result = await _useCase.ExecuteAsync(productId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain(productId.ToString());
        }
    }
}
