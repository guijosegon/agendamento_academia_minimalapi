using AgendamentoAcademia.API.Domain.Entities;
using AgendamentoAcademia.API.Application.Services;
using Microsoft.AspNetCore.Mvc;
using static AgendamentoAcademia.API.Application.DTOs.AgendamentoDTOs;
using AgendamentoAcademia.API.Infra;
using static AgendamentoAcademia.API.Application.DTOs.AulaDTOs;
using Microsoft.EntityFrameworkCore;

namespace AgendamentoAcademia.API.Api.Endpoints
{
    public static class AgendamentoEndpoints
    {
        public static IEndpointRouteBuilder MapAgendamentos(this IEndpointRouteBuilder app)
        {
            var map = app.MapGroup("api/agendamentos").WithTags("Agendamentos");

            map.MapPost("/", async ([FromBody] AgendamentoRequest request, AgendamentoService service) =>
            {
                var agendamento = new Agendamento();

                try
                {
                    agendamento = await service.BookAsync(request.AlunoId, request.AulaId, CancellationToken.None);

                    return Results.Created($"/agendamentos/{agendamento.Id}", new AgendamentoResponse(agendamento.Id, agendamento.AlunoId, agendamento.AulaId, agendamento.DataHora));
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new { code = "BUSINESS_RULE", message = ex.Message });
                }
            })
            .Produces<AgendamentoResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

            map.MapGet("/{id:int}", async (int id, AcademiaDbContext db) =>
            {
                var agendamento = await db.Agendamentos.AsQueryable().FirstOrDefaultAsync(f => f.Id == id);

                return agendamento is null ? Results.NotFound() : Results.Ok(new AgendamentoResponse(agendamento.Id, agendamento.AlunoId, agendamento.AulaId, agendamento.DataHora));
            })
            .Produces<AulaResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            map.MapDelete("/{id:int}", async (int id, AcademiaDbContext db) =>
            {
                var agendamento = await db.Agendamentos.AsTracking().FirstOrDefaultAsync(f => f.Id == id);

                if (agendamento is not null)
                    db.Remove(agendamento);

                return agendamento is null ? Results.NotFound() : Results.Ok();
            })
            .Produces<AulaResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            return app;
        }
    }
}