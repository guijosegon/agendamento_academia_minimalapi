using AgendamentoAcademia.API.Domain.Entities;
using AgendamentoAcademia.API.Domain.Extensions;
using AgendamentoAcademia.API.Infra;
using Microsoft.EntityFrameworkCore;
using static AgendamentoAcademia.API.Application.DTOs.AgendamentoDTOs;

namespace AgendamentoAcademia.API.Application.Services
{
    public class AgendamentoService
    {
        private readonly AcademiaDbContext _db;
        public AgendamentoService(AcademiaDbContext db) => _db = db;

        public async Task<Agendamento> BookAsync(int alunoId, int aulaId, CancellationToken cancellationToken = default)
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            var aluno = await _db.Alunos.AsQueryable().FirstOrDefaultAsync(f => f.Id == alunoId, cancellationToken) ?? throw new InvalidOperationException("Aluno não encontrado.");
            var aula = await _db.Aulas.Include(i => i.Agendamentos).FirstOrDefaultAsync(f => f.Id == aulaId, cancellationToken) ?? throw new InvalidOperationException("Aula não encontrada.");

            if (await _db.Agendamentos.AnyAsync(a => a.AlunoId == alunoId && a.AulaId == aulaId, cancellationToken))
                throw new InvalidOperationException("Aluno já está agendado nesta aula.");

            if (aula.Agendamentos.Count >= aula.Capacidade)
                throw new InvalidOperationException("Capacidade máxima da aula atingida.");

            var ano = aula.DataHora.Year;
            var mes = aula.DataHora.Month;

            var quantidadeMes = await _db.Agendamentos
            .Include(i => i.Aula)
            .Where(w => w.AlunoId == alunoId && w.Aula.DataHora.Year == ano && w.Aula.DataHora.Month == mes)
            .CountAsync(cancellationToken);

            var limiteMes = GetLimitePorPlano(aluno.Plano);

            if (quantidadeMes >= limiteMes)
                throw new InvalidOperationException($"Limite mensal do plano ({limiteMes}) atingido.");

            var agendamento = new Agendamento
            {
                AlunoId = aluno.Id,
                AulaId = aula.Id,
                DataHora = DateTime.UtcNow
            };

            _db.Agendamentos.Add(agendamento);
            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return agendamento;
        }

        public async Task<RelatorioMensalPorAluno> ReportAsync(int alunoId, int ano, int mes, CancellationToken cancellationToken = default)
        {
            var aluno = await _db.Alunos.FirstOrDefaultAsync(f => f.Id == alunoId, cancellationToken) ?? throw new InvalidOperationException("Aluno não encontrado.");

            var agendamentos = _db.Agendamentos
            .Include(i => i.Aula)
            .Where(w => w.AlunoId == alunoId && w.Aula.DataHora.Year == ano && w.Aula.DataHora.Month == mes);

            var total = await agendamentos.CountAsync(cancellationToken);

            var groups = await agendamentos
            .GroupBy(g => g.Aula.Tipo)
            .Select(s => new { Tipo = s.Key, Quantidade = s.Count() })
            .OrderByDescending(o => o.Quantidade)
            .ToListAsync(cancellationToken);

            var fator = total == 0 ? 0 : 100.0 / total;

            var aulasFrequentes = groups
                .Select(s => new AulaFrequenteItem(
                    Tipo: s.Tipo.ToString(),
                    Quantidade: s.Quantidade,
                    Porcentagem: Math.Round(s.Quantidade * fator, 2, MidpointRounding.AwayFromZero)))
                .ToList();

            return new RelatorioMensalPorAluno(aluno.Id, aluno.Nome, ano, mes, total, aulasFrequentes);
        }

        public static int GetLimitePorPlano(Enums.TipoPlano plano)
        {
            return plano switch
            {
                Enums.TipoPlano.Mensal => 12,
                Enums.TipoPlano.Trimestral => 20,
                Enums.TipoPlano.Anual => 30,
                _ => throw new NotImplementedException()
            };
        }
    }
}