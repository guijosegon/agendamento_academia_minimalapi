using AgendamentoAcademia.API.Domain.Extensions;

namespace AgendamentoAcademia.API.Domain.Entities
{
    public class Aula
    {
        public int Id { get; set; }
        public Enums.TipoAula Tipo { get; set; }
        public DateTime DataHora { get; set; }
        public int Capacidade { get; set; }
        public List<Agendamento> Agendamentos { get; set; }
    }
}