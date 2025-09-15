namespace GymClass.Models
{
    public class Aula
    {
        public int Id { get; set; }
        public string TipoAula { get; set; } = string.Empty; // Cross, Funcional, Pilates, etc.
        public DateTime DataHora { get; set; }
        public int CapacidadeMaxima { get; set; }
        public int VagasOcupadas { get; set; } = 0;
        public bool Ativa { get; set; } = true;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();

        public bool TemVagasDisponiveis() => VagasOcupadas < CapacidadeMaxima;

        public int VagasDisponiveis() => CapacidadeMaxima - VagasOcupadas;
    }
}
