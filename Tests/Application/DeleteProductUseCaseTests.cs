using Application.Interfaces;
using Application.UseCases.Products;
using Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace Tests.Application
{
    public class DeleteProductUseCaseTests
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DeleteProductUseCase _useCase;

        public DeleteProductUseCaseTests()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _useCase = new DeleteProductUseCase(_productRepository, _unitOfWork);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Delete_Product_When_It_Exists()
        {
            var productId = Guid.NewGuid();
            var existingProduct = new Product(productId, "Product Name", "CODE001", "", 100m, 80m);

            _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>())
                .Returns(existingProduct);

            var result = await _useCase.ExecuteAsync(productId);

            result.IsSuccess.Should().BeTrue();

            await _productRepository.Received(1).DeleteAsync(productId, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
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

            await _productRepository.DidNotReceive().DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
            await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }
    }
}
