
using DonationHub.Application.Campaigns.Commands.CreateCampaign;
using DonationHub.Application.Campaigns.Commands.DeleteCampaign;
using DonationHub.Application.Campaigns.Commands.UpdateCampaign;
using DonationHub.Application.Campaigns.DTOs;
using DonationHub.Application.Campaigns.Queries.GetCampaignById;
using DonationHub.Application.Campaigns.Queries.GetCampaigns;
using DonationHub.Application.Campaigns.Queries.GetSuccessStories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DonationHub.Api.Controllers;

[ApiController]
[Route("api/v1/campaigns")]
public class CampaignsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CampaignsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string GetLanguage()
    {
        var language = Request.Headers.AcceptLanguage
            .ToString()
            .Split(',')
            .FirstOrDefault()?
            .Split(';')
            .FirstOrDefault()?
            .Trim();

        return language?.Equals(
            "ar",
            StringComparison.OrdinalIgnoreCase) == true
                ? "ar"
                : "en";
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCampaigns(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetCampaignsQuery(GetLanguage()),
            cancellationToken);

        if (!result.IsSuccess)
            return HandleError(result.Error, result.ValidationErrors);

        return Ok(result.Value);
    }

    [HttpGet("success-stories")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSuccessStories(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetSuccessStoriesQuery(GetLanguage()),
            cancellationToken);

        if (!result.IsSuccess)
            return HandleError(result.Error, result.ValidationErrors);

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCampaignById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetCampaignByIdQuery(id, GetLanguage()),
            cancellationToken);

        if (!result.IsSuccess)
            return HandleError(result.Error, result.ValidationErrors);

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCampaign(
        [FromBody] CreateCampaignDto dto,
        CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirst(
            ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized();

        var result = await _mediator.Send(
            new CreateCampaignCommand(dto, userId),
            cancellationToken);

        if (!result.IsSuccess)
            return HandleError(result.Error, result.ValidationErrors);

        return CreatedAtAction(
            nameof(GetCampaignById),
            new { id = result.Value!.Id },
            result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCampaign(
        Guid id,
        [FromBody] UpdateCampaignDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateCampaignCommand(id, dto),
            cancellationToken);

        if (!result.IsSuccess)
            return HandleError(result.Error, result.ValidationErrors);

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCampaign(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DeleteCampaignCommand(id),
            cancellationToken);

        if (!result.IsSuccess)
            return HandleError(result.Error, result.ValidationErrors);

        return NoContent();
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
