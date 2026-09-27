using Application.Commands.Products;
using Application.Interfaces;
using Application.Models;
using Application.UseCases.Products;
using Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace Tests.Application
{
    public class CreateProductUseCaseTests
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CreateProductUseCase _useCase;

        public CreateProductUseCaseTests()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _useCase = new CreateProductUseCase(_productRepository, _unitOfWork);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Create_Product_When_Code_Is_Unique()
        {
            var command = new CreateProductCommand
            {
                Name = "Product Name",
                Code = "CODE001",
                Ean = "7891234567890",
                Price = 100m,
                PromotionalPrice = 80m
            };

            _productRepository.GetByCodeAsync(command.Code, Arg.Any<CancellationToken>())
                .Returns((Product?)null);

            var result = await _useCase.ExecuteAsync(command);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Name.Should().Be(command.Name);
            result.Value.Code.Should().Be(command.Code);
            result.Value.Ean.Should().Be(command.Ean);
            result.Value.Price.Should().Be(command.Price);
            result.Value.PromotionalPrice.Should().Be(command.PromotionalPrice);

            await _productRepository.Received(1).UpsertAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_Should_Return_Failure_When_Code_Already_Exists()
        {
            var command = new CreateProductCommand
            {
                Name = "Product Name",
                Code = "CODE001",
                Ean = "",
                Price = 100m,
                PromotionalPrice = 80m
            };

            var existingProduct = new Product(Guid.NewGuid(), "Existing", "CODE001", "", 50m, 40m);
            _productRepository.GetByCodeAsync(command.Code, Arg.Any<CancellationToken>())
                .Returns(existingProduct);

            var result = await _useCase.ExecuteAsync(command);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("CODE001");

            await _productRepository.DidNotReceive().UpsertAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
            await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_Should_Return_Failure_When_Domain_Validation_Fails()
        {
            var command = new CreateProductCommand
            {
                Name = "",
                Code = "CODE001",
                Ean = "",
                Price = 100m,
                PromotionalPrice = 80m
            };

            _productRepository.GetByCodeAsync(command.Code, Arg.Any<CancellationToken>())
                .Returns((Product?)null);

            var result = await _useCase.ExecuteAsync(command);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNullOrWhiteSpace();

            await _productRepository.DidNotReceive().UpsertAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
            await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }
    }
}
