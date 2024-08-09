using RangoAgil.API.EndpointFilters;
using RangoAgil.API.EndpointHandlers;

namespace RangoAgil.API.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void RegisterRangosEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        
        var rangosEndpoints = endpointRouteBuilder.MapGroup("/rangos");
        var rangosComIdEndpoints = rangosEndpoints.MapGroup("{rangoId:int}");
        var rangosComIdAndLockFilterEndpoints = rangosComIdEndpoints
            .MapGroup("")
            .AddEndpointFilter(new RangoIsLockedFilter(11))
            .AddEndpointFilter(new RangoIsLockedFilter(8));

        rangosEndpoints.MapGet("", RangosHandlers.GetRangosAsync);
        
        rangosEndpoints.MapPost("", RangosHandlers.CreateRangoAsync)
            .AddEndpointFilter<ValidateAnnotationFilter>();

        rangosComIdEndpoints.MapGet("", RangosHandlers.GetRangoIdAsync).WithName("GetRangos");

        rangosComIdAndLockFilterEndpoints.MapPut("", RangosHandlers.UpdateRangoAsync);

        rangosComIdAndLockFilterEndpoints.MapDelete("", RangosHandlers.DeleteRangoAsync)
            .AddEndpointFilter<LogNotFoundResponseFilter>();
    }

    public static void RegisterIngredientesEndpointes(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var ingredientesEndpoints = endpointRouteBuilder.MapGroup("/rangos/{rangoId:int}/ingredientes");
        
        ingredientesEndpoints.MapGet("", IngredientesHandlers.GetIngredientesAsync);

        ingredientesEndpoints.MapPost("", () =>
        {
            throw new NotImplementedException();
        });
    }
}
