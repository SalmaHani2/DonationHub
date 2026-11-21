using DonationHub.Application.Auth.Auth;
using DonationHub.Application.Auth.Options;
using DonationHub.Application.Common;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Identity;
using DonationHub.Application.Tokens;
using DonationHub.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace DonationHub.Application.Auth.Commands.Refresh;

public class RefreshAsyncHandler
    : IRequestHandler<RefreshAsyncCommand, Result<LoginResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWorks _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly JwtOptions _jwtOptions;
    private readonly TimeProvider _timeProvider;

    public RefreshAsyncHandler(
        IIdentityService identityService,
        IUnitOfWorks unitOfWork,
        ITokenService tokenService,
        TimeProvider timeProvider,
        IOptions<JwtOptions> jwtOptions)
    {
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
        _tokenService = tokenService;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<Result<LoginResponse>> Handle(
        RefreshAsyncCommand request,
        CancellationToken cancellationToken)
    {
        var tokenHash = _tokenService.HashRefreshToken(
            request.Request.RefreshToken);

        var oldToken = await _unitOfWork.Repository<RefreshToken>()
            .FirstOrDefaultAsync(
                token => token.TokenHash == tokenHash,
                cancellationToken);

       
        var user = await _identityService.FindByIdAsync(
            oldToken.UserId, cancellationToken);

       


        var accessToken = _tokenService.GenerateAccessToken(
            user.Id,
            user.Email,
            user.Roles);

        var newRefreshToken = _tokenService.GenerateRefreshToken();

        var newTokenEntity = new RefreshToken(user.Id,_tokenService.HashRefreshToken(newRefreshToken), _timeProvider.GetUtcNow().UtcDateTime,
        _timeProvider.GetUtcNow().UtcDateTime.AddDays(_jwtOptions.RefreshTokenExpirationDays));

        oldToken.Revoke(
            newTokenEntity.Id,
            _timeProvider.GetUtcNow().UtcDateTime);

        await _unitOfWork.Repository<RefreshToken>()
            .AddAsync(newTokenEntity, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<LoginResponse>.Success(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken
        });
    }
}