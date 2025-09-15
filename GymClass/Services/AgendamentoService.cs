using GymClass.DTOs;
using GymClass.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GymClass.Services
{
    public class AgendamentoService : IAgendamentoService
    {
        private readonly GymContext _context;
        private readonly ILogger<AgendamentoService> _logger;

        public AgendamentoService(GymContext context, ILogger<AgendamentoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Aluno> CreateAlunoAsync(AlunoCreateDto alunoDto)
        {
            var aluno = new Aluno
            {
                Nome = alunoDto.Nome,
                TipoPlano = alunoDto.TipoPlano,
                DataCriacao = DateTime.UtcNow,
                Ativo = true
            };

            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            return aluno;
        }

        public async Task<Aula> CreateAulaAsync(AulaCreateDto aulaDto)
        {
            if (!Validator.TryValidateObject(aulaDto, new ValidationContext(aulaDto), null))
                throw new ArgumentException("Dados da aula inválidos");

            var aula = new Aula
            {
                TipoAula = aulaDto.TipoAula,
                DataHora = aulaDto.DataHora,
                CapacidadeMaxima = aulaDto.CapacidadeMaxima,
                VagasOcupadas = 0,
                Ativa = true,
                DataCriacao = DateTime.UtcNow
            };

            _context.Aulas.Add(aula);
            await _context.SaveChangesAsync();

            return aula;
        }

        public async Task<Agendamento> CreateAgendamentoAsync(AgendamentoCreateDto agendamentoDto)
        {
            var aluno = await _context.Alunos.FindAsync(agendamentoDto.AlunoId);
            if (aluno == null || !aluno.Ativo)
                throw new InvalidOperationException("Aluno não encontrado ou inativo");

            var aula = await _context.Aulas.FindAsync(agendamentoDto.AulaId);
            if (aula == null || !aula.Ativa)
                throw new InvalidOperationException("Aula não encontrada ou inativa");

            if (!aula.TemVagasDisponiveis())
                throw new InvalidOperationException("Não há vagas disponíveis para esta aula");

            var agendamentosMes = await GetAgendamentosAlunoNoMes(aluno.Id, DateTime.UtcNow);
            var limite = GetLimiteAulasPorPlano(aluno.TipoPlano);

            if (agendamentosMes.Count >= limite)
                throw new InvalidOperationException($"Aluno atingiu o limite de {limite} aulas permitidas para o plano {aluno.TipoPlano}");
            var agendamentoExistente = await _context.Agendamentos
                .FirstOrDefaultAsync(a => a.AlunoId == aluno.Id && a.AulaId == aula.Id && a.Ativo);

            if (agendamentoExistente != null)
                throw new InvalidOperationException("Aluno já está agendado para esta aula");

            var agendamento = new Agendamento
            {
                AlunoId = aluno.Id,
                AulaId = aula.Id,
                DataAgendamento = DateTime.UtcNow,
                Ativo = true
            };

            aula.VagasOcupadas++;

            _context.Agendamentos.Add(agendamento);
            await _context.SaveChangesAsync();

            return agendamento;
        }

        public async Task<List<Aluno>> GetAlunosAsync()
        {
            return await _context.Alunos.Where(a => a.Ativo).ToListAsync();
        }

        public async Task<List<Aula>> GetAulasAsync()
        {
            return await _context.Aulas.Where(a => a.Ativa).ToListAsync();
        }

        public async Task<List<Agendamento>> GetAgendamentosAsync()
        {
            return await _context.Agendamentos
                .Include(a => a.Aluno)
                .Include(a => a.Aula)
                .Where(a => a.Ativo)
                .ToListAsync();
        }

        public async Task<RelatorioAlunoDto> GetRelatorioAlunoAsync(int alunoId)
        {
            var aluno = await _context.Alunos.FindAsync(alunoId);
            if (aluno == null || !aluno.Ativo)
                throw new InvalidOperationException("Aluno não encontrado ou inativo");

            var agendamentosMes = await GetAgendamentosAlunoNoMes(alunoId, DateTime.UtcNow);

            var frequenciaPorTipo = agendamentosMes
                .GroupBy(a => a.Aula.TipoAula)
                .Select(g => new TipoAulaFrequencia
                {
                    TipoAula = g.Key,
                    Quantidade = g.Count()
                })
                .OrderByDescending(t => t.Quantidade)
                .ToList();

            var relatorio = new RelatorioAlunoDto
            {
                AlunoId = aluno.Id,
                AlunoNome = aluno.Nome,
                TipoPlano = aluno.TipoPlano,
                TotalAulasAgendadasMes = agendamentosMes.Count,
                LimiteAulasMes = GetLimiteAulasPorPlano(aluno.TipoPlano),
                TiposAulaFrequencia = frequenciaPorTipo
            };

            return relatorio;
        }

        public async Task<bool> CancelarAgendamentoAsync(int agendamentoId)
        {
            var agendamento = await _context.Agendamentos
                .Include(a => a.Aula)
                .FirstOrDefaultAsync(a => a.Id == agendamentoId && a.Ativo);

            if (agendamento == null)
                return false;

            agendamento.Ativo = false;

            agendamento.Aula.VagasOcupadas--;

            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<List<Agendamento>> GetAgendamentosAlunoNoMes(int alunoId, DateTime? dataReferenciaUtc = null)
        {
            var dataUtc = dataReferenciaUtc ?? DateTime.UtcNow;

            var primeiroDiaMesUtc = new DateTime(dataUtc.Year, dataUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc);
 
            var primeiroDiaProximoMesUtc = primeiroDiaMesUtc.AddMonths(1);

            return await _context.Agendamentos
                .Where(a => a.AlunoId == alunoId &&
                           a.Ativo &&
                           a.DataAgendamento >= primeiroDiaMesUtc &&
                           a.DataAgendamento < primeiroDiaProximoMesUtc)
                .AsNoTracking()
                .ToListAsync();
        }

        private int GetLimiteAulasPorPlano(PlanoType plano)
        {
            return plano switch
            {
                PlanoType.Mensal => 12,
                PlanoType.Trimestral => 20,
                PlanoType.Anual => 30,
                _ => 0
            };
        }
    }
}
