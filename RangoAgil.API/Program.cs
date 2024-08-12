using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Contexto de banco de dados
builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"]));

// Adicionando a utiliza��o do EntityFramework no contexto de banco de dados da API em quest�o
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<RangoDbContext>();

// Mapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Controle e manipu��o de exce��es
builder.Services.AddProblemDetails();

// Controle de Seguran�a e autentica��o
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdmimFromBrazil", policy =>
        policy
            .RequireRole("admin")
            .RequireClaim("country", "Brazil"));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("TokenAuthRango",
        new()
        {
            Name = "Authorization",
            Description = "Token baseado em Autentica��o e Autoriza��o",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            In = ParameterLocation.Header
        });

    options.AddSecurityRequirement(new()
        {
            {
                new()
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "TokenAuthRango"
                    }
                },
                new List<string>()
            }
        });
});

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    /*
    app.UseExceptionHandler(configureApplicationBuider =>
    {
        configureApplicationBuider.Run(
            async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("An unexpected problem happened");
            });
    });
    */
}

/*
 Redireciona sempra para HTTPS.
 Se n�o funcionar tente deletar o certificado digital e executar novamente.
 */
app.UseHttpsRedirection();

// Uso do controle de seguran�a e autentica��o
app.UseAuthentication();
app.UseAuthorization();

// Usar a IDE do Swagger quando estiver no ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.RegisterRangosEndpoints();
app.RegisterIngredientesEndpointes();

app.Run();
