using Application.Dtos;
using Application.Interfaces;
using Application.Models;

namespace Application.UseCases.Products
{
    public class ListProductsUseCase
    {
        private readonly IProductRepository _productRepository;

        public ListProductsUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<PagedResultDto<ProductDto>> ExecuteAsync(int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var result = await _productRepository.GetAllAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResultDto<ProductDto>
            {
                Items = result.Items.Select(MapToDto).ToList().AsReadOnly(),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
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
