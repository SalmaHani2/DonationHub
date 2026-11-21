using DonationHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Common
{
    public interface IUnitOfWorks
    {
        IGenericRepository<T> Repository<T>() where T : Entity;
        Task<bool> TrySaveChangesAsync(CancellationToken cancellationToken);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
