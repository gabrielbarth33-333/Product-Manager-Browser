using Application.Interfaces;
using Infrastructure.Context;

namespace Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProjectDbContext _context;

        public UnitOfWork(ProjectDbContext context)
        {
            _context = context;
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
