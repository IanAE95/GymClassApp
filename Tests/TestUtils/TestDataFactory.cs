using GymClass.Models;

namespace GymClass.Tests.TestUtils
{
    public static class TestDataFactory
    {
        public static Aluno CriarAluno(int id = 1, string nome = "Teste Aluno", PlanoType plano = PlanoType.Mensal)
        {
            return new Aluno
            {
                Id = id,
                Nome = nome,
                TipoPlano = plano,
                DataCriacao = DateTime.UtcNow,
                Ativo = true
            };
        }

        public static Aula CriarAula(int id = 1, string tipo = "Yoga", int capacidade = 10, int vagasOcupadas = 0)
        {
            return new Aula
            {
                Id = id,
                TipoAula = tipo,
                DataHora = DateTime.UtcNow.AddHours(24),
                CapacidadeMaxima = capacidade,
                VagasOcupadas = vagasOcupadas,
                Ativa = true,
                DataCriacao = DateTime.UtcNow
            };
        }

        public static Agendamento CriarAgendamento(int id = 1, int alunoId = 1, int aulaId = 1)
        {
            return new Agendamento
            {
                Id = id,
                AlunoId = alunoId,
                AulaId = aulaId,
                DataAgendamento = DateTime.UtcNow,
                Ativo = true
            };
        }
    }
}
