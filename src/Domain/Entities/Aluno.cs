using AgendamentoAcademia.API.Domain.Extensions;

namespace AgendamentoAcademia.API.Domain.Entities
{
    public class Aluno
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public Enums.TipoPlano Plano { get; set; }
    }
}