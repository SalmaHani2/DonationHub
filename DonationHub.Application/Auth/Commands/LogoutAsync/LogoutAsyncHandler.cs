using DonationHub.Application.Common;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Tokens;
using DonationHub.Domain.Entities;
using MediatR;

namespace DonationHub.Application.Auth.Commands.Logout;

public class LogoutAsyncHandler
    : IRequestHandler<LogoutAsyncCommand, Result<bool>>
{
    private readonly IUnitOfWorks _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly TimeProvider _timeProvider;
    public LogoutAsyncHandler(
        IUnitOfWorks unitOfWork,
        ITokenService tokenService,
        TimeProvider timeProvider)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _timeProvider = timeProvider;
    }

    public async Task<Result<bool>> Handle(
        LogoutAsyncCommand request,
        CancellationToken cancellationToken)
    {
        var tokenHash = _tokenService.HashRefreshToken(
            request.RefreshToken);

        var refreshToken = await _unitOfWork.Repository<RefreshToken>()
            .FirstOrDefaultAsync(
                token =>
                    token.UserId == request.UserId &&
                    token.TokenHash == tokenHash,
                cancellationToken);

        if (refreshToken == null ||
            refreshToken.RevokedAt != null)
        {
            return Result<bool>.Success(true);
        }

        refreshToken.Revoke(
            null,
            _timeProvider.GetUtcNow().UtcDateTime);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}