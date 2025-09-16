using AgendamentoAcademia.API.Domain.Entities;
using AgendamentoAcademia.API.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AgendamentoAcademia.API.Domain.Extensions.Enums;
using static AgendamentoAcademia.API.Application.DTOs.AulaDTOs;

namespace AgendamentoAcademia.API.Api.Endpoints
{
    public static class AulaEndpoints
    {
        public static IEndpointRouteBuilder MapAulas(this IEndpointRouteBuilder app)
        {
            var map = app.MapGroup("/aulas").WithTags("Aulas");

            map.MapPost("/", async ([FromBody] AulaRequest request, AcademiaDbContext db) =>
            {
                var aula = new Aula
                {
                    Tipo = request.Tipo,
                    DataHora = request.Data,
                    Capacidade = request.Capacidade
                };

                db.Aulas.Add(aula);
                await db.SaveChangesAsync();

                return Results.Created($"/aulas/{aula.Id}", new AulaResponse(aula.Id, aula.Tipo, aula.DataHora, aula.Capacidade));
            });

            map.MapGet("/{id:int}", async (int id, AcademiaDbContext db) =>
            {
                var aula = await db.Aulas.AsQueryable().FirstOrDefaultAsync(f => f.Id == id);

                return aula is null ? Results.NotFound() : Results.Ok(new AulaResponse(aula.Id, aula.Tipo, aula.DataHora, aula.Capacidade));
            });

            map.MapGet("/", async ([FromQuery] DateTime? dataHotaInicio, [FromQuery] DateTime? dataHotaFim, [FromQuery] TipoAula? tipo, AcademiaDbContext db) =>
            {
                var aulas = db.Aulas.AsQueryable();

                if (dataHotaInicio.HasValue) aulas = aulas.Where(w => w.DataHora >= dataHotaInicio.Value);
                if (dataHotaFim.HasValue) aulas = aulas.Where(w => w.DataHora <= dataHotaFim.Value);
                if (tipo.HasValue) aulas = aulas.Where(w => w.Tipo == tipo.Value);

                var list = await aulas
                    .OrderBy(o => o.DataHora)
                    .Select(s => new AulaResponse(s.Id, s.Tipo, s.DataHora, s.Capacidade))
                    .ToListAsync();

                return Results.Ok(list);
            });

            return app;
        }
    }
}