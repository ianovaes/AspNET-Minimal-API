using MiniValidation;
using RangoAgil.API.Models;
using RangoAgil.API.Profiles;

namespace RangoAgil.API.EndpointFilters;

public class ValidateAnnotationFilter : IEndpointFilter
{
    async ValueTask<object?> IEndpointFilter.InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var rangoParaCriacaoDTO = context.GetArgument<RangoParaCriacaoDTO>(2);

        if (!MiniValidator.TryValidate(rangoParaCriacaoDTO, out var validationErrors))
        {
            return TypedResults.ValidationProblem(validationErrors);
        }

        return await next(context);
    }
}
