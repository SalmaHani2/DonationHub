using DonationHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq.Expressions;

namespace DonationHub.Application.Common
{
    public interface IGenericRepository<T> where T : Entity
    {
        Task AddAsync(T entity, CancellationToken cancellationToken);

        Task UpdateAsync(T entity, CancellationToken cancellationToken);

        Task RemoveAsync(T entity, CancellationToken cancellationToken);

        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate,CancellationToken cancellationToken);
        Task<T?> GetByIdWithIncludesAsync(Guid id,CancellationToken cancellationToken,params Expression<Func<T, object>>[] includes);
        void SetOriginalRowVersion(
            T entity,
            byte[] rowVersion);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
    }
}
