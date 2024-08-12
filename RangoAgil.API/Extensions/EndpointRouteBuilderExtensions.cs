using Microsoft.AspNetCore.Identity;
using RangoAgil.API.EndpointFilters;
using RangoAgil.API.EndpointHandlers;

namespace RangoAgil.API.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void RegisterRangosEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {

        endpointRouteBuilder.MapGroup("/identity/").MapIdentityApi<IdentityUser>();
        
        endpointRouteBuilder.MapGet("/ptatos/{pratoid:int}", (int pratoid) => $"O prato {pratoid} é delicioso!")
                .WithOpenApi(operation =>
                {
                    operation.Deprecated = true;
                    return operation;
                })
                .WithSummary("Este endpoint está deprecated e será descontinuado na versão 2 desta API")
                .WithDescription("Por favor utilize a outra rota desta API sendo ela /rangos/{rangoId} para evitar maiores transtornos futuros");

        var rangosEndpoints = endpointRouteBuilder.MapGroup("/rangos")
            .RequireAuthorization();
        var rangosComIdEndpoints = rangosEndpoints.MapGroup("{rangoId:int}");
        
        var rangosComIdAndLockFilterEndpoints = rangosComIdEndpoints
            .MapGroup("")
            .RequireAuthorization("RequireAdmimFromBrazil")
            .AddEndpointFilter(new RangoIsLockedFilter(11))
            .AddEndpointFilter(new RangoIsLockedFilter(8));

        rangosEndpoints.MapGet("", RangosHandlers.GetRangosAsync)
            .WithOpenApi()
            .WithSummary("Esta rota Retornará todos os Rangos");
        
        rangosEndpoints.MapPost("", RangosHandlers.CreateRangoAsync)
            .AddEndpointFilter<ValidateAnnotationFilter>();

        rangosComIdEndpoints.MapGet("", RangosHandlers.GetRangoIdAsync)
            .WithName("GetRangos")
            .AllowAnonymous();

        rangosComIdAndLockFilterEndpoints.MapPut("", RangosHandlers.UpdateRangoAsync);

        rangosComIdAndLockFilterEndpoints.MapDelete("", RangosHandlers.DeleteRangoAsync)
            .AddEndpointFilter<LogNotFoundResponseFilter>();
    }

    public static void RegisterIngredientesEndpointes(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var ingredientesEndpoints = endpointRouteBuilder.MapGroup("/rangos/{rangoId:int}/ingredientes")
            .RequireAuthorization();
        
        ingredientesEndpoints.MapGet("", IngredientesHandlers.GetIngredientesAsync);

        ingredientesEndpoints.MapPost("", () =>
        {
            throw new NotImplementedException();
        });
    }
}
