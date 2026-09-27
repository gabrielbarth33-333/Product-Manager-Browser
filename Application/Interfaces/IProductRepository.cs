using Application.Models;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IProductRepository
    {
        Task UpsertAsync(Product product, CancellationToken cancellationToken = default);
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<PagedResult<Product>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
