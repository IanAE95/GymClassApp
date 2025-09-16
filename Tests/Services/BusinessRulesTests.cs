using GymClass.DTOs;
using GymClass.Models;
using GymClass.Services;
using GymClass.Tests.TestUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GymClass.Tests.Services
{
    public class RegrasNegocioTests : IDisposable
    {
        private readonly DbContextOptions<GymContext> _dbContextOptions;
        private readonly Mock<ILogger<AgendamentoService>> _loggerMock;
        private GymContext _context;

        public RegrasNegocioTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<GymContext>()
                .UseInMemoryDatabase(databaseName: $"RegrasTest_{Guid.NewGuid()}")
                .Options;

            _loggerMock = new Mock<ILogger<AgendamentoService>>();
            _context = new GymContext(_dbContextOptions);
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task DeveBloquearAgendamento_QuandoDuplicado()
        {
            var aluno = TestDataFactory.CriarAluno();
            var aula = TestDataFactory.CriarAula();

            _context.Alunos.Add(aluno);
            _context.Aulas.Add(aula);
            _context.Agendamentos.Add(TestDataFactory.CriarAgendamento());

            await _context.SaveChangesAsync();

            var service = new AgendamentoService(_context, _loggerMock.Object);
            var agendamentoDto = new AgendamentoCreateDto { AlunoId = 1, AulaId = 1 };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateAgendamentoAsync(agendamentoDto));
        }

        [Fact]
        public async Task DeveBloquearAgendamento_QuandoAulaInativa()
        {
            var aluno = TestDataFactory.CriarAluno();
            var aula = TestDataFactory.CriarAula();
            aula.Ativa = false;

            _context.Alunos.Add(aluno);
            _context.Aulas.Add(aula);
            await _context.SaveChangesAsync();

            var service = new AgendamentoService(_context, _loggerMock.Object);
            var agendamentoDto = new AgendamentoCreateDto { AlunoId = 1, AulaId = 1 };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateAgendamentoAsync(agendamentoDto));
        }

        [Fact]
        public async Task DeveBloquearAgendamento_QuandoAlunoInativo()
        {
            var aluno = TestDataFactory.CriarAluno();
            aluno.Ativo = false;
            var aula = TestDataFactory.CriarAula();

            _context.Alunos.Add(aluno);
            _context.Aulas.Add(aula);
            await _context.SaveChangesAsync();

            var service = new AgendamentoService(_context, _loggerMock.Object);
            var agendamentoDto = new AgendamentoCreateDto { AlunoId = 1, AulaId = 1 };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateAgendamentoAsync(agendamentoDto));
        }
    }
}