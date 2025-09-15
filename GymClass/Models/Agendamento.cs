namespace GymClass.Models
{
    public class Agendamento
    {
        public int Id { get; set; }
        public int AlunoId { get; set; }
        public int AulaId { get; set; }
        public DateTime DataAgendamento { get; set; } = DateTime.UtcNow;
        public bool Ativo { get; set; } = true;

        // Navigation properties
        public virtual Aluno Aluno { get; set; } = null!;
        public virtual Aula Aula { get; set; } = null!;
    }
}
