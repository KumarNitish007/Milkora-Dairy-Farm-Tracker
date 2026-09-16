using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Milkora.Application.Common;

namespace Milkora.API.Filters;

/// <summary>
/// Global action filter that runs any registered FluentValidation validator for
/// each action argument. On failure it short-circuits with a 400 ApiResponse
/// carrying per-field errors — so controllers never contain validation code.
/// </summary>
public sealed class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var services = context.HttpContext.RequestServices;

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (services.GetService(validatorType) is not IValidator validator) continue;

            var result = await validator.ValidateAsync(new ValidationContext<object>(argument));
            if (result.IsValid) continue;

            var errors = result.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            context.Result = new BadRequestObjectResult(
                ApiResponse.Fail("One or more validation errors occurred.", errors));
            return;
        }

        await next();
    }
}
