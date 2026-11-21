
using System.Security.Claims;
using DonationHub.Application.Auth.Auth;
using DonationHub.Application.Auth.Commands.Login;
using DonationHub.Application.Auth.Commands.Logout;
using DonationHub.Application.Auth.Commands.Refresh;
using DonationHub.Application.Auth.Commands.Register;
using DonationHub.Application.Auth.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST: api/v1/auth/register
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterAsyncCommand
        {
            Request = request
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleError(result.Error, result.ValidationErrors);

        return Ok(result.Value);
    }

    // POST: api/v1/auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginAsyncCommand(request);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleError(result.Error, result.ValidationErrors);

        return Ok(result.Value);
    }

    // POST: api/v1/auth/refresh
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RefreshAsyncCommand(request);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleError(result.Error, result.ValidationErrors);

        return Ok(result.Value);
    }

    // POST: api/v1/auth/logout
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(
        [FromBody] RefreshRequest request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var command = new LogoutAsyncCommand
        {
            UserId = userId,
            RefreshToken = request.RefreshToken
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleError(result.Error, result.ValidationErrors);

        return NoContent();
    }

    // GET: api/v1/auth/me
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser(
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var query = new GetCurrentUserQuery
        {
            UserId = userId
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleError(result.Error, result.ValidationErrors);

        return Ok(result.Value);
    }

    private IActionResult HandleError(
        DonationHub.Application.Common.Errors.Error? error,
        Dictionary<string, string[]>? validationErrors = null)
    {
        if (error is null)
            return StatusCode(StatusCodes.Status500InternalServerError);

        var statusCode = error.Type.ToStatusCode();

        var problem = new ProblemDetails
        {
            Type = $"https://httpstatuses.io/{statusCode}",
            Title = error.Message,
            Status = statusCode
        };

        problem.Extensions["code"] = error.Code;
        problem.Extensions["traceId"] = HttpContext.TraceIdentifier;

        if (validationErrors is not null)
            problem.Extensions["errors"] = validationErrors;

        return new ObjectResult(problem)
        {
            StatusCode = statusCode,
            ContentTypes = { "application/problem+json" }
        };
    }
}
