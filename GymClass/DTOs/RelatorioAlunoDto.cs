using GymClass.Models;

namespace GymClass.DTOs
{
    public class RelatorioAlunoDto
    {
        public int AlunoId { get; set; }
        public string AlunoNome { get; set; } = string.Empty;
        public PlanoType TipoPlano { get; set; }
        public int TotalAulasAgendadasMes { get; set; }
        public int LimiteAulasMes { get; set; }
        public List<TipoAulaFrequencia> TiposAulaFrequencia { get; set; } = new List<TipoAulaFrequencia>();
    }

    public class TipoAulaFrequencia
    {
        public string TipoAula { get; set; } = string.Empty;
        public int Quantidade { get; set; }
    }
}