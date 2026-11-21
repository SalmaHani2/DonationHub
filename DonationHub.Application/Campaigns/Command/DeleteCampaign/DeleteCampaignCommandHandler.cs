using DonationHub.Application.Common;
using DonationHub.Application.Common.Errors;
using DonationHub.Application.Common.Results;
using DonationHub.Domain.Entities;
using MediatR;

namespace DonationHub.Application.Campaigns.Commands.DeleteCampaign;

public class DeleteCampaignCommandHandler
    : IRequestHandler<DeleteCampaignCommand, Result>
{
    private readonly IUnitOfWorks _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public DeleteCampaignCommandHandler(
        IUnitOfWorks unitOfWork,
        TimeProvider timeProvider)
    {
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<Result> Handle(
        DeleteCampaignCommand request,
        CancellationToken cancellationToken)
    {
        var campaign = await _unitOfWork.Repository<Campaign>()
            .GetByIdAsync(request.Id, cancellationToken);

        if (campaign == null)
        {
            return Result.Failure(ErrorCatalog.Campaign.NotFound);
        }

        campaign.Deleted(_timeProvider.GetUtcNow().UtcDateTime);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}