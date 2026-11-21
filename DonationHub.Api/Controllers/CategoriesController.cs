
using DonationHub.Application.Categories.Categories;
using DonationHub.Application.Categories.Commands.CreateAsync;
using DonationHub.Application.Categories.Commands.DeleteAsync;
using DonationHub.Application.Categories.Commands.UpdateAsync;
using DonationHub.Application.Categories.Query.GetAllAsync;
using DonationHub.Application.Categories.Query.GetByIdAsync;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers;

[ApiController]
[Route("api/v1/categories")]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/v1/categories
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAllAsyncQuery(),
            cancellationToken);

        if (!result.IsSuccess)
            return HandleError(result.Error, result.ValidationErrors);

        return Ok(result.Value);
    }

    // GET: api/v1/categories/{id}
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetByIdAsyncQuery
        {
            Id = id
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleError(result.Error, result.ValidationErrors);

        return Ok(result.Value);
    }

    // POST: api/v1/categories
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand
        {
            Request = request
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleError(result.Error, result.ValidationErrors);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            result.Value);
    }

    // PUT: api/v1/categories/{id}
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAsyncCommand
        {
            Id = id,
            Request = request
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleError(result.Error, result.ValidationErrors);

        return Ok(result.Value);
    }

    // DELETE: api/v1/categories/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteAsyncCommand
        {
            Id = id
        };

        var result = await _mediator.Send(command, cancellationToken);

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
