using RangoAgil.API.EndpointHandlers;

namespace RangoAgil.API.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void RegisterRangosEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        
        var rangosEndpoints = endpointRouteBuilder.MapGroup("/rangos");
        var rangosComIdEndpoints = rangosEndpoints.MapGroup("{rangoId:int}");

        rangosEndpoints.MapGet("", RangosHandlers.GetRangosAsync);
        
        rangosEndpoints.MapPost("", RangosHandlers.CreateRangoAsync);

        rangosComIdEndpoints.MapGet("", RangosHandlers.GetRangoIdAsync).WithName("GetRangos");
        
        rangosComIdEndpoints.MapPut("", RangosHandlers.UpdateRangoAsync);
        
        rangosComIdEndpoints.MapDelete("", RangosHandlers.DeleteRangoAsync);
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
