using GymClass.DTOs;
using GymClass.Models;
using GymClass.Services;
using Microsoft.AspNetCore.Mvc;

namespace GymClass.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgendamentosController : ControllerBase
    {
        private readonly IAgendamentoService _agendamentoService;

        public AgendamentosController(IAgendamentoService agendamentoService)
        {
            _agendamentoService = agendamentoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AgendamentoDto>>> GetAgendamentos()
        {
            var agendamentos = await _agendamentoService.GetAgendamentosAsync();
            var agendamentoDtos = agendamentos.Select(a => new AgendamentoDto
            {
                Id = a.Id,
                AlunoId = a.AlunoId,
                AlunoNome = a.Aluno.Nome,
                AulaId = a.AulaId,
                AulaTipo = a.Aula.TipoAula,
                AulaDataHora = a.Aula.DataHora,
                DataAgendamento = a.DataAgendamento,
                Ativo = a.Ativo
            });

            return Ok(agendamentoDtos);
        }

        [HttpPost]
        public async Task<ActionResult<AgendamentoDto>> CreateAgendamento(AgendamentoCreateDto agendamentoDto)
        {
            try
            {
                var agendamento = await _agendamentoService.CreateAgendamentoAsync(agendamentoDto);

                var agendamentoCompleto = await _agendamentoService.GetAgendamentosAsync();
                var agendamentoCriado = agendamentoCompleto.FirstOrDefault(a => a.Id == agendamento.Id);

                if (agendamentoCriado == null)
                    return NotFound();

                var result = new AgendamentoDto
                {
                    Id = agendamentoCriado.Id,
                    AlunoId = agendamentoCriado.AlunoId,
                    AlunoNome = agendamentoCriado.Aluno.Nome,
                    AulaId = agendamentoCriado.AulaId,
                    AulaTipo = agendamentoCriado.Aula.TipoAula,
                    AulaDataHora = agendamentoCriado.Aula.DataHora,
                    DataAgendamento = agendamentoCriado.DataAgendamento,
                    Ativo = agendamentoCriado.Ativo
                };

                return CreatedAtAction(nameof(GetAgendamentos), new { id = agendamento.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelarAgendamento(int id)
        {
            try
            {
                var result = await _agendamentoService.CancelarAgendamentoAsync(id);

                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpGet("relatorio/aluno/{alunoId}")]
        public async Task<ActionResult<RelatorioAlunoDto>> GetRelatorioAluno(int alunoId)
        {
            try
            {
                var relatorio = await _agendamentoService.GetRelatorioAlunoAsync(alunoId);
                return Ok(relatorio);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
    }
}
