using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"]));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

var rangosEndpoints = app.MapGroup("/rangos");
var rangosComIdEndpoints = rangosEndpoints.MapGroup("{rangoId:int}");
var ingredientesEndpoints = rangosComIdEndpoints.MapGroup("/ingredientes");

rangosEndpoints.MapGet("", async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>>(RangoDbContext rangoDb, IMapper mapper, [FromQuery(Name = "name")] string? rangoNome) =>
{
    var rangosEntity =  await rangoDb.Rangos
        .Where(x => rangoNome == null || x.Nome.ToLower().Contains(rangoNome.ToLower()))
        .ToListAsync();

    if (rangosEntity.Count <= 0 || rangosEntity == null)
    {
        return TypedResults.NoContent();
    }
    
    return TypedResults.Ok(mapper.Map<IEnumerable<RangoDTO>>(rangosEntity));
});

ingredientesEndpoints.MapGet("", async Task<Results<NoContent, Ok<IEnumerable<IngredienteDTO>>>>(
    RangoDbContext rangoDb,
    IMapper mapper,
    int rangoId) =>
{
    var rangosEntity = await rangoDb.Rangos
        .Include(rango => rango.Ingredientes)
        .FirstOrDefaultAsync(rango => rango.Id == rangoId);

    if (rangosEntity?.Ingredientes.Count <= 0 || (rangosEntity?.Ingredientes ?? null) == null)
    {
        return TypedResults.NoContent();
    }

    return TypedResults.Ok(mapper.Map<IEnumerable<IngredienteDTO>>(rangosEntity?.Ingredientes));
});

rangosComIdEndpoints.MapGet("", async Task<Results<NotFound, Ok<RangoDTO>>> (
    RangoDbContext rangoDb,
    IMapper mapper,
    int rangoId) =>
{
    var rangoReturn = await rangoDb.Rangos.FirstOrDefaultAsync(rango => rango.Id == rangoId);

    if (rangoReturn == null)
    {
        return TypedResults.NotFound();
    }

    return TypedResults.Ok(mapper.Map<RangoDTO>(rangoReturn));
}).WithName("GetRangos");

rangosEndpoints.MapPost("", async Task<CreatedAtRoute<RangoDTO>> (RangoDbContext rangoDb,
    IMapper mapper,
    [FromBody] RangoParaCriacaoDTO rangoParaCriacaoDTO
    //LinkGenerator linkGenerator,
    //HttpContext httpContext
    ) => 
{
    var rangoEntity = mapper.Map<Rango>(rangoParaCriacaoDTO);
    rangoDb.Add(rangoEntity);
    await rangoDb.SaveChangesAsync();

    var rangoToReturn = mapper.Map<RangoDTO>(rangoEntity);

    return TypedResults.CreatedAtRoute(rangoToReturn,
        "GetRangos",
        new { rangoId = rangoToReturn.Id });

    // Referência para Alunos
    //var linkToReturn = linkGenerator.GetUriByName(
    //    httpContext,
    //    "GetRango",
    //    new {id = rangoToReturn.Id});
    
    //return TypedResults.Created(linkToReturn, rangoToReturn);
});

rangosComIdEndpoints.MapPut("", async Task<Results<NotFound, Ok>> (
    RangoDbContext rangoDb,
    IMapper mapper,
    int rangoId,
    [FromBody] RangoParaAtualizacaoDTO rangoParaAtualizacaoDTO) =>
{
    var rangoEntity = await rangoDb.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

    if (rangoEntity == null)
    {
        return TypedResults.NotFound();
    }

    mapper.Map(rangoParaAtualizacaoDTO, rangoEntity);

    await rangoDb.SaveChangesAsync();

    return TypedResults.Ok();
    
});

rangosComIdEndpoints.MapDelete("", async Task<Results<NotFound, NoContent>> (
    RangoDbContext rangoDb,
    int rangoId) =>
{
    var rangoEntity = await rangoDb.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

    if (rangoEntity == null)
    {
        return TypedResults.NotFound();
    }

    rangoDb.Rangos.Remove(rangoEntity);

    await rangoDb.SaveChangesAsync();

    return TypedResults.NoContent();
});

app.Run();
