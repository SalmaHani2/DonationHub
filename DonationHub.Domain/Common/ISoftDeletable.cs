using DonationHub.Application.Common.Results;
using DonationHub.Domain.Common;

namespace DonationHub.Domain.Common;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAtUtc { get; }

    Result Deleted(DateTime deletedAt);

    Result UndoDelete();
}