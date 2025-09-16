using AgendamentoAcademia.API.Infra;
using AgendamentoAcademia.API.Api.Endpoints;
using AgendamentoAcademia.API.Application.Services;
using AgendamentoAcademia.API.Infra.Seeders;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using AgendamentoAcademia.API.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AcademiaDbContext>(opt =>opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<AgendamentoService>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SchemaFilter<EnumSchemaFilter>();
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(opt =>
{
    opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AcademiaDbContext>();
    await db.Database.EnsureCreatedAsync();
    await db.AlunoSeedAsync();
    await db.AulaSeedAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapAlunos();
app.MapAulas();
app.MapAgendamentos();

app.Run();