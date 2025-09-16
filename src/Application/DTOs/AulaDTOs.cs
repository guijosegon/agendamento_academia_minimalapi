using AgendamentoAcademia.API.Domain.Extensions;
using System.ComponentModel.DataAnnotations;

namespace AgendamentoAcademia.API.Application.DTOs
{
    public class AulaDTOs
    {
        public record AulaRequest(
            [property: Required] Enums.TipoAula Tipo,
            [property: Required] DateTime Data,
            [property: Range(1, 500)] int Capacidade
        );

        public record AulaResponse(
            int Id,
            Enums.TipoAula Tipo,
            DateTime Data,
            int Capacidade
        );
    }
}