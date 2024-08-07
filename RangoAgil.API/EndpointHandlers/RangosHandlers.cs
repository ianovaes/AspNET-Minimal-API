using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;

namespace RangoAgil.API.EndpointHandlers;

public static class RangosHandlers
{
    public static async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>> GetRangosAsync
    (RangoDbContext rangoDb,
    IMapper mapper,
    ILogger<RangoDTO> logger,
    [FromQuery(Name = "name")] string? rangoNome)
    {
        var rangoEntity = await rangoDb.Rangos
            .Where(x => rangoNome == null || x.Nome.ToLower().Contains(rangoNome.ToLower()))
            .ToListAsync();

        if (rangoEntity.Count <= 0 || rangoEntity == null)
        {
            logger.LogWarning($"Rango não encontrado. Parâmetro: {rangoNome}");
            return TypedResults.NoContent();
        }

        logger.LogInformation($"Retornando o Rango encontrado. Parâmetro: {rangoNome}");
        return TypedResults.Ok(mapper.Map<IEnumerable<RangoDTO>>(rangoEntity));

    }

    public static async Task<Results<NotFound, Ok<RangoDTO>>> GetRangoIdAsync(
        RangoDbContext rangoDb,
        IMapper mapper,
        int rangoId)
    {
        var rangoReturn = await rangoDb.Rangos.FirstOrDefaultAsync(rango => rango.Id == rangoId);

        if (rangoReturn == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(mapper.Map<RangoDTO>(rangoReturn));
    }

    public static async Task<CreatedAtRoute<RangoDTO>> CreateRangoAsync(
        RangoDbContext rangoDb,
        IMapper mapper,
        [FromBody] RangoParaCriacaoDTO rangoParaCriacaoDTO)
    {
        var rangoEntity = mapper.Map<Rango>(rangoParaCriacaoDTO);
        rangoDb.Add(rangoEntity);
        await rangoDb.SaveChangesAsync();

        var rangoToReturn = mapper.Map<RangoDTO>(rangoEntity);

        return TypedResults.CreatedAtRoute(rangoToReturn,
            "GetRangos",
            new { rangoId = rangoToReturn.Id });
    }

    public static async Task<Results<NotFound, Ok>> UpdateRangoAsync (
        RangoDbContext rangoDb,
        IMapper mapper,
        int rangoId,
        [FromBody] RangoParaAtualizacaoDTO rangoParaAtualizacaoDTO)
    {
        var rangoEntity = await rangoDb.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

        if (rangoEntity == null)
        {
            return TypedResults.NotFound();
        }

        mapper.Map(rangoParaAtualizacaoDTO, rangoEntity);

        await rangoDb.SaveChangesAsync();

        return TypedResults.Ok();
    }

    public static async Task<Results<NotFound, NoContent>> DeleteRangoAsync(
        RangoDbContext rangoDb,
        int rangoId)
    {
        var rangoEntity = await rangoDb.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

        if (rangoEntity == null)
        {
            return TypedResults.NotFound();
        }

        rangoDb.Rangos.Remove(rangoEntity);

        await rangoDb.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
