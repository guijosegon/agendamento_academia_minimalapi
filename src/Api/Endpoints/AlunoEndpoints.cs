using AgendamentoAcademia.API.Domain.Entities;
using AgendamentoAcademia.API.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            });

            map.MapGet("/{id:int}", async (int id, AcademiaDbContext db) =>
            {
                var aluno = await db.Alunos.AsQueryable().FirstOrDefaultAsync(f => f.Id == id);
                return aluno is null ? Results.NotFound() : Results.Ok(new AlunoResponse(aluno.Id, aluno.Nome, aluno.Plano));
            });

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
            });

            return app;
        }
    }
}