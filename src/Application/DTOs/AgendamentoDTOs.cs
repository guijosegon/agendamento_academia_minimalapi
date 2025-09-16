namespace AgendamentoAcademia.API.Application.DTOs
{
    public class AgendamentoDTOs
    {
        public record AgendamentoRequest(int AlunoId, int AulaId);
        public record AgendamentoResponse(int Id, int AlunoId, int AulaId, DateTime Date);

        public record RelatorioMensalPorAluno(
            int AlunoId,
            string AlunoNome,
            int Ano,
            int Mes,
            int TotalAgendamentos,
            IReadOnlyList<(string Tipo, int Quantidade, double Porcentagem)> AulasFrequentes
        );
    }
}