using Application.Dtos;
using Application.Interfaces;
using Application.Models;

namespace Application.UseCases.Products
{
    public class GetProductByIdUseCase
    {
        private readonly IProductRepository _productRepository;

        public GetProductByIdUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Result<ProductDto>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);

            if (product is null)
                return Result<ProductDto>.Failure($"Produto com id '{id}' não encontrado.");

            return Result<ProductDto>.Success(MapToDto(product));
        }

        private static ProductDto MapToDto(Domain.Entities.Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Code = product.Code,
                Ean = product.Ean,
                Price = product.Price,
                PromotionalPrice = product.PromotionalPrice
            };
        }
    }
}
