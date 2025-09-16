namespace AgendamentoAcademia.API.Application.DTOs
{
    public class AgendamentoDTOs
    {
        public record AgendamentoRequest(int AlunoId, int AulaId);
        public record AgendamentoResponse(int Id, int AlunoId, int AulaId, DateTimeOffset DataHora);
        public record AulaFrequenteItem(string Tipo, int Quantidade, double Porcentagem);

        public record RelatorioMensalPorAluno(
            int AlunoId,
            string AlunoNome,
            int Ano,
            int Mes,
            int TotalAgendamentos,
            IReadOnlyList<AulaFrequenteItem> AulasFrequentes
        );
    }
}