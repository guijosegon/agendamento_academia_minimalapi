using AgendamentoAcademia.API.Domain.Extensions;
using System.ComponentModel.DataAnnotations;

namespace AgendamentoAcademia.API.Application.DTOs
{
    public class AlunoDTOs
    {
        public record AlunoRequest([property: Required, MaxLength(150)] string Nome, [property: Required] Enums.TipoPlano Plano);
        public record AlunoResponse(int Id, string Nome, Enums.TipoPlano Plano);
    }
}