using AgendamentoAcademia.API.Domain.Entities;
using AgendamentoAcademia.API.Application.Services;
using Microsoft.AspNetCore.Mvc;
using static AgendamentoAcademia.API.Application.DTOs.AgendamentoDTOs;

namespace AgendamentoAcademia.API.Api.Endpoints
{
    public static class AgendamentoEndpoints
    {
        public static IEndpointRouteBuilder MapAgendamentos(this IEndpointRouteBuilder app)
        {
            var map = app.MapGroup("/agendamentos").WithTags("Agendamentos");

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
            });

            return app;
        }
    }
}