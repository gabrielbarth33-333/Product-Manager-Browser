using Application.Commands.Products;
using Application.Dtos;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;

namespace Application.UseCases.Products
{
    public class CreateProductUseCase
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductUseCase(
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ProductDto>> ExecuteAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingProduct = await _productRepository.GetByCodeAsync(command.Code, cancellationToken);
                if (existingProduct is not null)
                    return Result<ProductDto>.Failure($"Já existe um produto com o código '{command.Code}'.");

                var product = new Product(
                    Guid.NewGuid(),
                    command.Name,
                    command.Code,
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

        private static ProductDto MapToDto(Product product)
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
