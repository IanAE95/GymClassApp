using System.ComponentModel.DataAnnotations;
using GymClass.Validations;

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
        [Range(1, int.MaxValue)]
        public int CapacidadeMaxima { get; set; }

        public int VagasOcupadas { get; set; }
        public int VagasDisponiveis { get; set; }
        public bool Ativa { get; set; }
        public DateTime DataCriacao { get; set; }
    }

    public class AulaCreateDto
    {
        [Required(ErrorMessage = "Tipo da aula é obrigatório")]
        [StringLength(50)]
        public string TipoAula { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data/hora é obrigatória")]
        [FutureDate(MinutesOffset = 5, ErrorMessage = "Data/hora deve ser futura")]
        public DateTime DataHora { get; set; }

        [Required(ErrorMessage = "Capacidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacidade deve ser maior que 0")]
        public int CapacidadeMaxima { get; set; }
    }
}