using System.ComponentModel.DataAnnotations;

namespace GymClass.DTOs
{
    public class AgendamentoDto
    {
        public int Id { get; set; }
        public int AlunoId { get; set; }
        public string AlunoNome { get; set; } = string.Empty;
        public int AulaId { get; set; }
        public string AulaTipo { get; set; } = string.Empty;
        public DateTime AulaDataHora { get; set; }
        public DateTime DataAgendamento { get; set; }
        public bool Ativo { get; set; }
    }

    public class AgendamentoCreateDto
    {
        [Required]
        public int AlunoId { get; set; }

        [Required]
        public int AulaId { get; set; }
    }
}
