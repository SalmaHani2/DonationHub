
using System.Diagnostics;
using DonationHub.Application.Common.Errors;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Extensions;

public static class ProblemDetailsExtensions
{
    public static ObjectResult ToProblemDetailsResult(
        this ControllerBase controller,
        Error error,
        Dictionary<string, string[]>? validationErrors = null)
    {
        var statusCode = error.Type.ToStatusCode();

        var traceId = Activity.Current?.Id
                      ?? controller.HttpContext.TraceIdentifier;

        var problemDetails = new ProblemDetails
        {
            Type = $"https://httpstatuses.io/{statusCode}",
            Title = error.Message,
            Status = statusCode,
            Instance = controller.HttpContext.Request.Path
        };

        // Add the error catalogue code.
        problemDetails.Extensions["code"] = error.Code;

        // Add the request trace ID.
        problemDetails.Extensions["traceId"] = traceId;

        // Add property-level errors for validation failures only.
        if (error.Type == ErrorType.Validation &&
            validationErrors is { Count: > 0 })
        {
            problemDetails.Extensions["errors"] = validationErrors;
        }

        var result = new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };

        result.ContentTypes.Add("application/problem+json");

        return result;
    }
}
