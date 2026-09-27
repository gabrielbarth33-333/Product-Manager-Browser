using Application.Commands.Products;
using Application.Interfaces;
using Application.UseCases.Products;
using Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace Tests.Application
{
    public class UpdateProductUseCaseTests
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UpdateProductUseCase _useCase;

        public UpdateProductUseCaseTests()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _useCase = new UpdateProductUseCase(_productRepository, _unitOfWork);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Update_Product_When_It_Exists()
        {
            var productId = Guid.NewGuid();
            var existingProduct = new Product(productId, "Old Name", "CODE001", "", 100m, 80m);
            var command = new UpdateProductCommand
            {
                Id = productId,
                Name = "New Name",
                Ean = "7891234567890",
                Price = 200m,
                PromotionalPrice = 150m
            };

            _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>())
                .Returns(existingProduct);

            var result = await _useCase.ExecuteAsync(command);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Name.Should().Be("New Name");
            result.Value.Ean.Should().Be("7891234567890");
            result.Value.Price.Should().Be(200m);
            result.Value.PromotionalPrice.Should().Be(150m);

            await _productRepository.Received(1).UpsertAsync(existingProduct, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_Should_Return_Failure_When_Product_Not_Found()
        {
            var command = new UpdateProductCommand
            {
                Id = Guid.NewGuid(),
                Name = "New Name",
                Ean = "",
                Price = 200m,
                PromotionalPrice = 150m
            };

            _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns((Product?)null);

            var result = await _useCase.ExecuteAsync(command);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain(command.Id.ToString());

            await _productRepository.DidNotReceive().UpsertAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
            await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_Should_Return_Failure_When_Domain_Validation_Fails()
        {
            var productId = Guid.NewGuid();
            var existingProduct = new Product(productId, "Old Name", "CODE001", "", 100m, 80m);
            var command = new UpdateProductCommand
            {
                Id = productId,
                Name = "",
                Ean = "",
                Price = 100m,
                PromotionalPrice = 80m
            };

            _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>())
                .Returns(existingProduct);

            var result = await _useCase.ExecuteAsync(command);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNullOrWhiteSpace();

            await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }
    }
}
