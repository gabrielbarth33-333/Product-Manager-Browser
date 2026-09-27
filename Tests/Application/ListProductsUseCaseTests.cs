using Application.Dtos;
using Application.Interfaces;
using Application.Models;
using Application.UseCases.Products;
using Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace Tests.Application
{
    public class ListProductsUseCaseTests
    {
        private readonly IProductRepository _productRepository;
        private readonly ListProductsUseCase _useCase;

        public ListProductsUseCaseTests()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _useCase = new ListProductsUseCase(_productRepository);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Return_Paged_Products()
        {
            var products = new List<Product>
            {
                new Product(Guid.NewGuid(), "Product A", "CODE001", "", 100m, 80m),
                new Product(Guid.NewGuid(), "Product B", "CODE002", "", 200m, 150m)
            };

            var pagedResult = new PagedResult<Product>(products, 2, 1, 20);

            _productRepository.GetAllAsync(1, 20, Arg.Any<CancellationToken>())
                .Returns(pagedResult);

            var result = await _useCase.ExecuteAsync(1, 20);

            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(20);
            result.TotalPages.Should().Be(1);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Return_Empty_When_No_Products()
        {
            var pagedResult = new PagedResult<Product>(new List<Product>(), 0, 1, 20);

            _productRepository.GetAllAsync(1, 20, Arg.Any<CancellationToken>())
                .Returns(pagedResult);

            var result = await _useCase.ExecuteAsync(1, 20);

            result.Items.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
            result.TotalPages.Should().Be(0);
        }
    }
}
