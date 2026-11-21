using DonationHub.Application.Auth.Auth;
using DonationHub.Application.Auth.Options;
using DonationHub.Application.Common;
using DonationHub.Application.Common.Errors;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Identity;
using DonationHub.Application.Tokens;
using DonationHub.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace DonationHub.Application.Auth.Commands.Login;

public class LoginAsyncHandler
    : IRequestHandler<LoginAsyncCommand, Result<LoginResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWorks _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly JwtOptions _jwtOptions;
    private readonly TimeProvider _timeProvider;
    public LoginAsyncHandler(
        IIdentityService identityService,
        IUnitOfWorks unitOfWork,
        ITokenService tokenService,
        TimeProvider timeProvider,
        IOptions<JwtOptions> jwtOptions)
    {
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _timeProvider = timeProvider;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<Result<LoginResponse>> Handle(
        LoginAsyncCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Request.Email.Trim();

        var user = await _identityService.FindByEmailAsync(
            email, cancellationToken);
        if (user == null)
        {
            return Result<LoginResponse>.Failure(
                ErrorCatalog.Auth.InvalidCredentials);
        }
        var isPasswordValid = await _identityService.CheckPasswordAsync(
        email,
             request.Request.Password,
           cancellationToken);

        if (!isPasswordValid)
        {
            return Result<LoginResponse>.Failure(
                ErrorCatalog.Auth.InvalidCredentials);
        }

        var accessToken = _tokenService.GenerateAccessToken(
            user.Id,
            user.Email,
            user.Roles);

        var refreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenHash =
            _tokenService.HashRefreshToken(refreshToken);

        var token = new RefreshToken(
                user.Id,
                refreshTokenHash, _timeProvider.GetUtcNow().UtcDateTime,
        _timeProvider.GetUtcNow().UtcDateTime.AddDays(
                    _jwtOptions.RefreshTokenExpirationDays));

        await _unitOfWork.Repository<RefreshToken>()
            .AddAsync(token, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<LoginResponse>.Success(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }
}