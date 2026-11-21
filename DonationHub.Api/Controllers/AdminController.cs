
using DonationHub.Application.Admin.Admin;
using DonationHub.Application.Admin.Commands.CreateStaff;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin,CampaignManager")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("staff")]
    public async Task<IActionResult> CreateStaff(
        [FromBody] CreateStaffRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateStaffAsyncCommand(request);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleError(result.Error, result.ValidationErrors);

        return Created(
            $"/api/v1/admin/staff/{result.Value!.UserId}",
            result.Value);
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
