
using DonationHub.Application.Common;
using DonationHub.Domain.Common;
using DonationHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace DonationHub.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : Entity
{
    private readonly AppDbContext _context;

    public GenericRepository(AppDbContext context)
    {
        _context = context ;
    }

    public async Task AddAsync(T entity,CancellationToken cancellationToken)
    {
        await _context.Set<T>().AddAsync(entity, cancellationToken);
    }

    public Task UpdateAsync(T entity,CancellationToken cancellationToken)
    {
        _context.Set<T>().Update(entity);
        return Task.CompletedTask;
    }
    public void SetOriginalRowVersion(
    T entity,
    byte[] rowVersion)
    {
        _context.Entry(entity)
            .Property("RowVersion")
            .OriginalValue = rowVersion;
    }

    public Task RemoveAsync(T entity, CancellationToken cancellationToken)
    {
        _context.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Set<T>().FindAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
    {

        return await _context.Set<T>().ToListAsync(cancellationToken);
    }
   
    public async Task<T?> GetByIdWithIncludesAsync(
       Guid id,
       CancellationToken cancellationToken,
       params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _context.Set<T>();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(
            x => x.Id == id,
            cancellationToken);
    }
    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate,CancellationToken cancellationToken)
    {
        return await _context.Set<T>()
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }

}
