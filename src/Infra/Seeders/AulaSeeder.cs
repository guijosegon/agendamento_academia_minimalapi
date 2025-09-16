using AgendamentoAcademia.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AgendamentoAcademia.API.Domain.Extensions;

namespace AgendamentoAcademia.API.Infra.Seeders
{
    public static class AulaSeeder
    {
        public static async Task AulaSeedAsync(this AcademiaDbContext db)
        {
            if (!await db.Aulas.AnyAsync())
            {
                var now = DateTime.Now;

                db.Aulas.AddRange(
                new Aula { Tipo = Enums.TipoAula.Cross, DataHora = now.AddDays(1).AddHours(9), Capacidade = 10 },
                new Aula { Tipo = Enums.TipoAula.Funcional, DataHora = now.AddDays(2).AddHours(18), Capacidade = 8 },
                new Aula { Tipo = Enums.TipoAula.Pilates, DataHora = now.AddDays(1).AddHours(7), Capacidade = 6 },
                new Aula { Tipo = Enums.TipoAula.Pilates, DataHora = now.AddDays(3).AddHours(7), Capacidade = 12 },
                new Aula { Tipo = Enums.TipoAula.Funcional, DataHora = now.AddDays(5).AddHours(11), Capacidade = 7 },
                new Aula { Tipo = Enums.TipoAula.Pilates, DataHora = now.AddDays(2).AddHours(2), Capacidade = 15 }
                );
            }

            await db.SaveChangesAsync();
        }
    }
}