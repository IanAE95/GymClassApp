using GymClass.Models;
using GymClass.DTOs;
using GymClass.Services;
using Microsoft.AspNetCore.Mvc;

namespace GymClass.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AulasController : ControllerBase
    {
        private readonly IAgendamentoService _agendamentoService;

        public AulasController(IAgendamentoService agendamentoService)
        {
            _agendamentoService = agendamentoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AulaDto>>> GetAulas()
        {
            var aulas = await _agendamentoService.GetAulasAsync();
            var aulaDtos = aulas.Select(a => new AulaDto
            {
                Id = a.Id,
                TipoAula = a.TipoAula,
                DataHora = a.DataHora,
                CapacidadeMaxima = a.CapacidadeMaxima,
                VagasOcupadas = a.VagasOcupadas,
                VagasDisponiveis = a.VagasDisponiveis(),
                Ativa = a.Ativa,
                DataCriacao = a.DataCriacao
            });

            return Ok(aulaDtos);
        }

        [HttpPost]
        public async Task<ActionResult<AulaDto>> CreateAula(AulaCreateDto aulaDto)
        {
            try
            {
                var aula = await _agendamentoService.CreateAulaAsync(aulaDto);
                var result = new AulaDto
                {
                    Id = aula.Id,
                    TipoAula = aula.TipoAula,
                    DataHora = aula.DataHora,
                    CapacidadeMaxima = aula.CapacidadeMaxima,
                    VagasOcupadas = aula.VagasOcupadas,
                    VagasDisponiveis = aula.VagasDisponiveis(),
                    Ativa = aula.Ativa,
                    DataCriacao = aula.DataCriacao
                };

                return CreatedAtAction(nameof(GetAulas), new { id = aula.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
