using Microsoft.AspNetCore.Mvc;
using SeamlessShareApi.Models.Response;

namespace SeamlessShareApi;

internal static class ActionContextExtensions
{
    public static BadRequestObjectResult Format(this ActionContext actionContext)
    {
        var errors = actionContext.ModelState
            .Where(kvp => kvp.Value is { Errors.Count: > 0 })
            .SelectMany(kvp => kvp.Value!.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        var errorMessage = string.Join(" ", errors);
        return new BadRequestObjectResult(new GenericMessage(errorMessage));
    }
}
