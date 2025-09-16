using AgendamentoAcademia.API.Domain.Entities;
using AgendamentoAcademia.API.Domain.Extensions;
using AgendamentoAcademia.API.Infra;
using AgendamentoAcademia.API.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace AgendamentoAcademia.Tests
{
    public class AgendamentoTests
    {
        private AcademiaDbContext NewAcademiaDbContext()
        {
            var options = new DbContextOptionsBuilder<AcademiaDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            return new AcademiaDbContext(options);
        }

        [Fact]
        public async Task NaoPermiteAgendarAcimaDaCapacidade()
        {
            using var db = NewAcademiaDbContext();
            var service = new AgendamentoService(db);

            var aluno1 = new Aluno { Nome = "Anual 1", Plano = Enums.TipoPlano.Anual };
            var aluno2 = new Aluno { Nome = "Anual 2", Plano = Enums.TipoPlano.Anual };
            var aula = new Aula { Tipo = Enums.TipoAula.Cross, DataHora = DateTimeOffset.Now, Capacidade = 1 };

            db.AddRange(aluno1, aluno2, aula);
            await db.SaveChangesAsync();

            await service.BookAsync(aluno1.Id, aula.Id);
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.BookAsync(aluno2.Id, aula.Id));
        }

        [Fact]
        public async Task RespeitaLimiteMensalPorPlano()
        {
            using var db = NewAcademiaDbContext();
            var service = new AgendamentoService(db);

            var aluno = new Aluno { Nome = "Mensal", Plano = Enums.TipoPlano.Mensal };
            db.Alunos.Add(aluno);

            for (int i = 0; i < 12; i++)
            {
                var aula = new Aula { Tipo = Enums.TipoAula.Funcional, DataHora = new DateTimeOffset(2025, 9, 30, 10, 0, 0, TimeSpan.Zero), Capacidade = 5 };
                db.Aulas.Add(aula);
                await db.SaveChangesAsync();
                await service.BookAsync(aluno.Id, aula.Id);
            }

            var extra = new Aula { Tipo = Enums.TipoAula.Funcional, DataHora = new DateTimeOffset(2025, 9, 30, 10, 0, 0, TimeSpan.Zero), Capacidade = 5 };
            db.Aulas.Add(extra);
            await db.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.BookAsync(aluno.Id, extra.Id));
        }


        [Fact]
        public async Task RelatorioRetornaTopTipos()
        {
            using var db = NewAcademiaDbContext();
            var service = new AgendamentoService(db);

            var aluno = new Aluno { Nome = "Carla", Plano = Enums.TipoPlano.Anual };
            var aula1 = new Aula { Tipo = Enums.TipoAula.Cross, DataHora = new DateTimeOffset(2025, 9, 1, 9, 0, 0, TimeSpan.Zero), Capacidade = 10 };
            var aula2 = new Aula { Tipo = Enums.TipoAula.Pilates, DataHora = new DateTimeOffset(2025, 9, 1, 9, 0, 0, TimeSpan.Zero), Capacidade = 10 };
            var aula3 = new Aula { Tipo = Enums.TipoAula.Pilates, DataHora = new DateTimeOffset(2025, 9, 1, 9, 0, 0, TimeSpan.Zero), Capacidade = 10 };

            db.AddRange(aluno, aula1, aula2, aula3);
            await db.SaveChangesAsync();

            await service.BookAsync(aluno.Id, aula1.Id);
            await service.BookAsync(aluno.Id, aula2.Id);
            await service.BookAsync(aluno.Id, aula3.Id);

            var report = await service.ReportAsync(aluno.Id, 2025, 9);
            Assert.Equal(3, report.TotalAgendamentos);
            Assert.Equal("Pilates", report.AulasFrequentes.First().Tipo);
        }
    }
}