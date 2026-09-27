using Application.Commands.Products;
using Application.Dtos;
using Application.Interfaces;
using Application.Models;

namespace Application.UseCases.Products
{
    public class UpdateProductUseCase
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductUseCase(
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ProductDto>> ExecuteAsync(UpdateProductCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(command.Id, cancellationToken);

                if (product is null)
                    return Result<ProductDto>.Failure($"Produto com id '{command.Id}' não encontrado.");

                product.UpdateDetails(
                    command.Name,
                    command.Ean,
                    command.Price,
                    command.PromotionalPrice);

                await _productRepository.UpsertAsync(product, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                return Result<ProductDto>.Success(MapToDto(product));
            }
            catch (ArgumentException ex)
            {
                return Result<ProductDto>.Failure(ex.Message);
            }
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
