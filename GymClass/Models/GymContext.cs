using Microsoft.EntityFrameworkCore;

namespace GymClass.Models
{
    public class GymContext : DbContext
    {
        public GymContext(DbContextOptions<GymContext> options) : base(options)
        {
        }
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Aula> Aulas { get; set; }
        public DbSet<Agendamento> Agendamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Aluno>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Nome).IsRequired().HasMaxLength(100);
                entity.Property(a => a.TipoPlano).IsRequired();
                entity.Property(a => a.DataCriacao).IsRequired();
                entity.Property(a => a.Ativo).IsRequired();
            });

            modelBuilder.Entity<Aula>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.TipoAula).IsRequired().HasMaxLength(50);
                entity.Property(a => a.DataHora).IsRequired();
                entity.Property(a => a.CapacidadeMaxima).IsRequired();
                entity.Property(a => a.VagasOcupadas).IsRequired();
                entity.Property(a => a.Ativa).IsRequired();
                entity.Property(a => a.DataCriacao).IsRequired();
            });

            modelBuilder.Entity<Agendamento>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.AlunoId).IsRequired();
                entity.Property(a => a.AulaId).IsRequired();
                entity.Property(a => a.DataAgendamento).IsRequired();
                entity.Property(a => a.Ativo).IsRequired();

                // Relacionamentos
                entity.HasOne(a => a.Aluno)
                      .WithMany(a => a.Agendamentos)
                      .HasForeignKey(a => a.AlunoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Aula)
                      .WithMany(a => a.Agendamentos)
                      .HasForeignKey(a => a.AulaId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Aluno>().HasData(
                new Aluno { Id = 1, Nome = "João Silva", TipoPlano = PlanoType.Mensal, DataCriacao = DateTime.UtcNow, Ativo = true },
                new Aluno { Id = 2, Nome = "Maria Santos", TipoPlano = PlanoType.Trimestral, DataCriacao = DateTime.UtcNow, Ativo = true },
                new Aluno { Id = 3, Nome = "Pedro Costa", TipoPlano = PlanoType.Anual, DataCriacao = DateTime.UtcNow, Ativo = true }
            );

            modelBuilder.Entity<Aula>().HasData(
                new Aula { Id = 1, TipoAula = "Cross", DataHora = DateTime.Today.AddDays(1).AddHours(10), CapacidadeMaxima = 20, VagasOcupadas = 0, Ativa = true, DataCriacao = DateTime.UtcNow },
                new Aula { Id = 2, TipoAula = "Funcional", DataHora = DateTime.Today.AddDays(1).AddHours(18), CapacidadeMaxima = 15, VagasOcupadas = 0, Ativa = true, DataCriacao = DateTime.UtcNow },
                new Aula { Id = 3, TipoAula = "Pilates", DataHora = DateTime.Today.AddDays(2).AddHours(9), CapacidadeMaxima = 10, VagasOcupadas = 0, Ativa = true, DataCriacao = DateTime.UtcNow },
                new Aula { Id = 4, TipoAula = "Cross", DataHora = DateTime.Today.AddDays(3).AddHours(11), CapacidadeMaxima = 20, VagasOcupadas = 0, Ativa = true, DataCriacao = DateTime.UtcNow }
            );
        }
    }
}
