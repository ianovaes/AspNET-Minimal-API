using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Models;

namespace RangoAgil.API.EndpointHandlers
{
    public static class IngredientesHandlers
    {
        public static async Task<Results<NoContent, Ok<IEnumerable<IngredienteDTO>>>> GetIngredientesAsync(
            RangoDbContext rangoDb,
            IMapper mapper,
            int rangoId)
        {
            var rangosEntity = await rangoDb.Rangos
                .Include(rango => rango.Ingredientes)
                .FirstOrDefaultAsync(rango => rango.Id == rangoId);

            if (rangosEntity?.Ingredientes.Count <= 0 || (rangosEntity?.Ingredientes ?? null) == null)
            {
                return TypedResults.NoContent();
            }

            return TypedResults.Ok(mapper.Map<IEnumerable<IngredienteDTO>>(rangosEntity?.Ingredientes));
        }
    }
}
