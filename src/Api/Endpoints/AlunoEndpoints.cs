using AgendamentoAcademia.API.Application.Services;
using AgendamentoAcademia.API.Domain.Entities;
using AgendamentoAcademia.API.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AgendamentoAcademia.API.Application.DTOs.AgendamentoDTOs;
using static AgendamentoAcademia.API.Application.DTOs.AlunoDTOs;

namespace AgendamentoAcademia.API.Api.Endpoints
{
    public static class AlunoEndpoints
    {
        public static IEndpointRouteBuilder MapAlunos(this IEndpointRouteBuilder app)
        {
            var map = app.MapGroup("/alunos").WithTags("Alunos");

            map.MapPost("/", async ([FromBody] AlunoRequest request, AcademiaDbContext db) =>
            {
                var aluno = new Aluno { Nome = request.Nome, Plano = request.Plano };

                db.Alunos.Add(aluno);
                await db.SaveChangesAsync();

                return Results.Created($"/alunos/{aluno.Id}", new AlunoResponse(aluno.Id, aluno.Nome, aluno.Plano));
            })
            .Produces<AlunoResponse>(StatusCodes.Status200OK);

            map.MapGet("/{id:int}", async (int id, AcademiaDbContext db) =>
            {
                var aluno = await db.Alunos.AsQueryable().FirstOrDefaultAsync(f => f.Id == id);
                return aluno is null ? Results.NotFound() : Results.Ok(new AlunoResponse(aluno.Id, aluno.Nome, aluno.Plano));
            })
            .Produces<AlunoResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            map.MapGet("/", async ([FromQuery] int skip, [FromQuery] int take, AcademiaDbContext db) =>
            {
                skip = Math.Max(0, skip);
                take = take is > 0 and <= 100 ? take : 20;

                var data = await db.Alunos
                    .OrderBy(o => o.Nome)
                    .Skip(skip)
                    .Take(take)
                    .Select(s => new AlunoResponse(s.Id, s.Nome, s.Plano))
                    .ToListAsync();

                return Results.Ok(data);
            })
            .Produces<List<AlunoResponse>>(StatusCodes.Status200OK);

            map.MapGet("/{id:int}/relatorio", async (int id, [FromQuery] int ano, [FromQuery] int mes, AgendamentoService service) =>
            {
                if (ano <= 0 || mes is < 1 or > 12)
                    return Results.BadRequest(new { code = "INVALID_RANGE", message = "Informe ano (20XX) e mes (1 até 12) válidos." });

                try
                {
                    var relatorio = await service.ReportAsync(id, ano, mes);
                    return Results.Ok(relatorio);
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("Aluno não encontrado", StringComparison.OrdinalIgnoreCase))
                {
                    return Results.NotFound(new { code = "NOT_FOUND", message = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new { code = "BUSINESS_RULE", message = ex.Message });
                }
            })
            .Produces<RelatorioMensalPorAluno>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

            return app;
        }
    }
}