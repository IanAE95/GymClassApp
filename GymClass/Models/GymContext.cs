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
        }
    }
}
