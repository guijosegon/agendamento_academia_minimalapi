using Microsoft.EntityFrameworkCore;
using AgendamentoAcademia.API.Domain.Entities;

namespace AgendamentoAcademia.API.Infra
{
    public class AcademiaDbContext(DbContextOptions<AcademiaDbContext> options) : DbContext(options)
    {
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Aula> Aulas { get; set; }
        public DbSet<Agendamento> Agendamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Aluno>().Property(s => s.Nome).HasMaxLength(150).IsRequired();
            modelBuilder.Entity<Agendamento>().HasIndex(b => new { b.AulaId, b.AlunoId }).IsUnique();

            modelBuilder.Entity<Agendamento>()
                .HasOne(b => b.Aluno)
                .WithMany()
                .HasForeignKey(b => b.AlunoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Agendamento>()
                .HasOne(b => b.Aula)
                .WithMany(c => c.Agendamentos)
                .HasForeignKey(b => b.AulaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}