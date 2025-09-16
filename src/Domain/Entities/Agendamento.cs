namespace AgendamentoAcademia.API.Domain.Entities
{
    public class Agendamento
    {
        public int Id { get; set; }
        public int AlunoId { get; set; }
        public Aluno Aluno { get; set; }
        public int AulaId { get; set; }
        public Aula Aula { get; set; }
        public DateTimeOffset DataHora { get; set; }
    }
}