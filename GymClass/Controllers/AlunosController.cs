using GymClass.DTOs;
using GymClass.Services;
using Microsoft.AspNetCore.Mvc;

namespace GymClass.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlunosController : ControllerBase
    {
        private readonly IAgendamentoService _agendamentoService;

        public AlunosController(IAgendamentoService agendamentoService)
        {
            _agendamentoService = agendamentoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlunoDto>>> GetAlunos()
        {
            var alunos = await _agendamentoService.GetAlunosAsync();
            var alunoDtos = alunos.Select(a => new AlunoDto
            {
                Id = a.Id,
                Nome = a.Nome,
                TipoPlano = a.TipoPlano,
                DataCriacao = a.DataCriacao,
                Ativo = a.Ativo
            });

            return Ok(alunoDtos);
        }

        [HttpPost]
        public async Task<ActionResult<AlunoDto>> CreateAluno(AlunoCreateDto alunoDto)
        {
            try
            {
                var aluno = await _agendamentoService.CreateAlunoAsync(alunoDto);
                var result = new AlunoDto
                {
                    Id = aluno.Id,
                    Nome = aluno.Nome,
                    TipoPlano = aluno.TipoPlano,
                    DataCriacao = aluno.DataCriacao,
                    Ativo = aluno.Ativo
                };

                return CreatedAtAction(nameof(GetAlunos), new { id = aluno.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}