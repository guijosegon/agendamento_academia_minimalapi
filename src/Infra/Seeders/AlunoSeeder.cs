using AgendamentoAcademia.API.Domain.Entities;
using AgendamentoAcademia.API.Infra;
using Microsoft.EntityFrameworkCore;
using AgendamentoAcademia.API.Domain.Extensions;

namespace AgendamentoAcademia.API.Infra.Seeders
{
    public static class AlunoSeeder
    {
        public static async Task AlunoSeedAsync(this AcademiaDbContext db)
        {
            if (!await db.Alunos.AnyAsync())
            {
                db.Alunos.AddRange(
                new Aluno { Nome = "Bruno", Plano = Enums.TipoPlano.Trimestral },
                new Aluno { Nome = "Ricardo", Plano = Enums.TipoPlano.Mensal },
                new Aluno { Nome = "Carla", Plano = Enums.TipoPlano.Anual },
                new Aluno { Nome = "Roberto", Plano = Enums.TipoPlano.Mensal },
                new Aluno { Nome = "Vitor", Plano = Enums.TipoPlano.Trimestral },
                new Aluno { Nome = "Paula", Plano = Enums.TipoPlano.Anual }
                );
            }

            await db.SaveChangesAsync();
        }
    }
}