using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProjectDbContext _context;
        private const int MaxPageSize = 20;

        public ProductRepository(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task UpsertAsync(Product product, CancellationToken cancellationToken = default)
        {
            var existing = await _context.Products
                .FirstOrDefaultAsync(p => p.Code == product.Code, cancellationToken);

            if (existing is null)
            {
                _context.Products.Add(product);
            }
            else
            {
                existing.UpdateDetails(
                    product.Name,
                    product.Ean,
                    product.Price,
                    product.PromotionalPrice);
            }
        }

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);
        }

        public async Task<PagedResult<Product>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

            var totalCount = await _context.Products.CountAsync(cancellationToken);

            var items = await _context.Products
                .AsNoTracking()
                .OrderBy(p => p.Code)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Product>(items, totalCount, pageNumber, pageSize);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _context.Products
                .FindAsync(new object[] { id }, cancellationToken);

            if (product is not null)
            {
                _context.Products.Remove(product);
            }
        }
    }
}
