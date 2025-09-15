using GymClass.Models;
using System.ComponentModel.DataAnnotations;

namespace GymClass.DTOs
{
    public class AlunoDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public PlanoType TipoPlano { get; set; }

        public DateTime DataCriacao { get; set; }
        public bool Ativo { get; set; }
    }

    public class AlunoCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public PlanoType TipoPlano { get; set; }
    }
}
