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
            _logger.LogInformation("AgendamentoService inicializado");
        }

        public async Task<Aluno> CreateAlunoAsync(AlunoCreateDto alunoDto)
        {
            _logger.LogInformation("Iniciando criação de aluno: {Nome}, Plano: {TipoPlano}",
                alunoDto.Nome, alunoDto.TipoPlano);

            try
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

                _logger.LogInformation("Aluno criado com sucesso. ID: {AlunoId}, Nome: {Nome}",
                    aluno.Id, aluno.Nome);

                return aluno;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar aluno: {Nome}, Plano: {TipoPlano}",
                    alunoDto.Nome, alunoDto.TipoPlano);
                throw;
            }
        }

        public async Task<Aula> CreateAulaAsync(AulaCreateDto aulaDto)
        {
            _logger.LogInformation("Iniciando criação de aula: {TipoAula}, Data: {DataHora}, Capacidade: {CapacidadeMaxima}",
                aulaDto.TipoAula, aulaDto.DataHora, aulaDto.CapacidadeMaxima);

            if (!Validator.TryValidateObject(aulaDto, new ValidationContext(aulaDto), null))
            {
                _logger.LogWarning("Dados da aula inválidos: {TipoAula}, Data: {DataHora}",
                    aulaDto.TipoAula, aulaDto.DataHora);
                throw new ArgumentException("Dados da aula inválidos");
            }

            try
            {
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

                _logger.LogInformation("Aula criada com sucesso. ID: {AulaId}, Tipo: {TipoAula}",
                    aula.Id, aula.TipoAula);

                return aula;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar aula: {TipoAula}, Data: {DataHora}",
                    aulaDto.TipoAula, aulaDto.DataHora);
                throw;
            }
        }

        public async Task<Agendamento> CreateAgendamentoAsync(AgendamentoCreateDto agendamentoDto)
        {
            _logger.LogInformation("Iniciando criação de agendamento. AlunoId: {AlunoId}, AulaId: {AulaId}",
                agendamentoDto.AlunoId, agendamentoDto.AulaId);

            try
            {
                var aluno = await _context.Alunos.FindAsync(agendamentoDto.AlunoId);
                if (aluno == null || !aluno.Ativo)
                {
                    _logger.LogWarning("Aluno não encontrado ou inativo. AlunoId: {AlunoId}",
                        agendamentoDto.AlunoId);
                    throw new InvalidOperationException("Aluno não encontrado ou inativo");
                }

                var aula = await _context.Aulas.FindAsync(agendamentoDto.AulaId);
                if (aula == null || !aula.Ativa)
                {
                    _logger.LogWarning("Aula não encontrada ou inativa. AulaId: {AulaId}",
                        agendamentoDto.AulaId);
                    throw new InvalidOperationException("Aula não encontrada ou inativa");
                }

                if (!aula.TemVagasDisponiveis())
                {
                    _logger.LogWarning("Não há vagas disponíveis para a aula. AulaId: {AulaId}, VagasOcupadas: {VagasOcupadas}, Capacidade: {Capacidade}",
                        aula.Id, aula.VagasOcupadas, aula.CapacidadeMaxima);
                    throw new InvalidOperationException("Não há vagas disponíveis para esta aula");
                }

                var agendamentosMes = await GetAgendamentosAlunoNoMes(aluno.Id, DateTime.UtcNow);
                var limite = GetLimiteAulasPorPlano(aluno.TipoPlano);

                if (agendamentosMes.Count >= limite)
                {
                    _logger.LogWarning("Aluno atingiu limite de aulas. AlunoId: {AlunoId}, Plano: {Plano}, AgendamentosMes: {Count}, Limite: {Limite}",
                        aluno.Id, aluno.TipoPlano, agendamentosMes.Count, limite);
                    throw new InvalidOperationException($"Aluno atingiu o limite de {limite} aulas permitidas para o plano {aluno.TipoPlano}");
                }

                var agendamentoExistente = await _context.Agendamentos
                    .FirstOrDefaultAsync(a => a.AlunoId == aluno.Id && a.AulaId == aula.Id && a.Ativo);

                if (agendamentoExistente != null)
                {
                    _logger.LogWarning("Aluno já está agendado para esta aula. AlunoId: {AlunoId}, AulaId: {AulaId}",
                        aluno.Id, aula.Id);
                    throw new InvalidOperationException("Aluno já está agendado para esta aula");
                }

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

                _logger.LogInformation("Agendamento criado com sucesso. AgendamentoId: {AgendamentoId}, Aluno: {AlunoNome}, Aula: {TipoAula}",
                    agendamento.Id, aluno.Nome, aula.TipoAula);

                return agendamento;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar agendamento. AlunoId: {AlunoId}, AulaId: {AulaId}",
                    agendamentoDto.AlunoId, agendamentoDto.AulaId);
                throw;
            }
        }

        public async Task<List<Aluno>> GetAlunosAsync()
        {
            _logger.LogInformation("Buscando lista de alunos ativos");

            try
            {
                var alunos = await _context.Alunos.Where(a => a.Ativo).ToListAsync();
                _logger.LogInformation("Encontrados {Count} alunos ativos", alunos.Count);
                return alunos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar lista de alunos");
                throw;
            }
        }

        public async Task<List<Aula>> GetAulasAsync()
        {
            _logger.LogInformation("Buscando lista de aulas ativas");

            try
            {
                var aulas = await _context.Aulas.Where(a => a.Ativa).ToListAsync();
                _logger.LogInformation("Encontradas {Count} aulas ativas", aulas.Count);
                return aulas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar lista de aulas");
                throw;
            }
        }

        public async Task<List<Agendamento>> GetAgendamentosAsync()
        {
            _logger.LogInformation("Buscando lista de agendamentos ativos");

            try
            {
                var agendamentos = await _context.Agendamentos
                    .Include(a => a.Aluno)
                    .Include(a => a.Aula)
                    .Where(a => a.Ativo)
                    .ToListAsync();

                _logger.LogInformation("Encontrados {Count} agendamentos ativos", agendamentos.Count);
                return agendamentos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar lista de agendamentos");
                throw;
            }
        }

        public async Task<RelatorioAlunoDto> GetRelatorioAlunoAsync(int alunoId)
        {
            _logger.LogInformation("Gerando relatório para alunoId: {AlunoId}", alunoId);

            try
            {
                var aluno = await _context.Alunos.FindAsync(alunoId);
                if (aluno == null || !aluno.Ativo)
                {
                    _logger.LogWarning("Aluno não encontrado ou inativo para relatório. AlunoId: {AlunoId}", alunoId);
                    throw new InvalidOperationException("Aluno não encontrado ou inativo");
                }

                var agendamentosMes = await GetAgendamentosAlunoNoMes(alunoId, DateTime.UtcNow);

                var frequenciaPorTipo = agendamentosMes
                    .GroupBy(a => a.Aula?.TipoAula ?? "Aula não encontrada")
                    .Where(g => g.Key != "Aula não encontrada")
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

                _logger.LogInformation("Relatório gerado com sucesso para aluno: {AlunoNome}, TotalAulas: {TotalAulas}",
                    aluno.Nome, agendamentosMes.Count);

                return relatorio;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório para alunoId: {AlunoId}", alunoId);
                throw;
            }
        }

        public async Task<bool> CancelarAgendamentoAsync(int agendamentoId)
        {
            _logger.LogInformation("Iniciando cancelamento de agendamento. AgendamentoId: {AgendamentoId}", agendamentoId);

            try
            {
                var agendamento = await _context.Agendamentos
                    .Include(a => a.Aula)
                    .FirstOrDefaultAsync(a => a.Id == agendamentoId && a.Ativo);

                if (agendamento == null)
                {
                    _logger.LogWarning("Agendamento não encontrado ou já cancelado. AgendamentoId: {AgendamentoId}", agendamentoId);
                    return false;
                }

                agendamento.Ativo = false;
                agendamento.Aula.VagasOcupadas--;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Agendamento cancelado com sucesso. AgendamentoId: {AgendamentoId}, AulaId: {AulaId}",
                    agendamentoId, agendamento.AulaId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar agendamento. AgendamentoId: {AgendamentoId}", agendamentoId);
                throw;
            }
        }

        private async Task<List<Agendamento>> GetAgendamentosAlunoNoMes(int alunoId, DateTime? dataReferenciaUtc = null)
        {
            _logger.LogDebug("Buscando agendamentos do mês para alunoId: {AlunoId}", alunoId);

            try
            {
                var dataUtc = dataReferenciaUtc ?? DateTime.UtcNow;
                var primeiroDiaMesUtc = new DateTime(dataUtc.Year, dataUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var primeiroDiaProximoMesUtc = primeiroDiaMesUtc.AddMonths(1);

                var agendamentos = await _context.Agendamentos
                    .Where(a => a.AlunoId == alunoId &&
                               a.Ativo &&
                               a.DataAgendamento >= primeiroDiaMesUtc &&
                               a.DataAgendamento < primeiroDiaProximoMesUtc)
                    .AsNoTracking()
                    .ToListAsync();

                _logger.LogDebug("Encontrados {Count} agendamentos para alunoId: {AlunoId} no mês",
                    agendamentos.Count, alunoId);

                return agendamentos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar agendamentos do mês para alunoId: {AlunoId}", alunoId);
                throw;
            }
        }

        private int GetLimiteAulasPorPlano(PlanoType plano)
        {
            var limite = plano switch
            {
                PlanoType.Mensal => 12,
                PlanoType.Trimestral => 20,
                PlanoType.Anual => 30,
                _ => 0
            };
            return limite;
        }
    }
}