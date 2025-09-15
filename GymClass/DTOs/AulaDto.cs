using System.ComponentModel.DataAnnotations;

namespace GymClass.DTOs
{
    public class AulaDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoAula { get; set; } = string.Empty;

        [Required]
        public DateTime DataHora { get; set; }

        [Required]
        [Range(1, 100)]
        public int CapacidadeMaxima { get; set; }

        public int VagasOcupadas { get; set; }
        public int VagasDisponiveis { get; set; }
        public bool Ativa { get; set; }
        public DateTime DataCriacao { get; set; }
    }

    public class AulaCreateDto
    {
        [Required]
        [StringLength(50)]
        public string TipoAula { get; set; } = string.Empty;

        [Required]
        public DateTime DataHora { get; set; }

        [Required]
        [Range(1, 100)]
        public int CapacidadeMaxima { get; set; }
    }
}