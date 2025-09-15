using GymClass.DTOs;
using GymClass.Models;

namespace GymClass.Services
{
    public interface IAgendamentoService
    {
        Task<Aluno> CreateAlunoAsync(AlunoCreateDto alunoDto);
        Task<Aula> CreateAulaAsync(AulaCreateDto aulaDto);
        Task<Agendamento> CreateAgendamentoAsync(AgendamentoCreateDto agendamentoDto);
        Task<List<Aluno>> GetAlunosAsync();
        Task<List<Aula>> GetAulasAsync();
        Task<List<Agendamento>> GetAgendamentosAsync();
        Task<RelatorioAlunoDto> GetRelatorioAlunoAsync(int alunoId);
        Task<bool> CancelarAgendamentoAsync(int agendamentoId);
    }
}
