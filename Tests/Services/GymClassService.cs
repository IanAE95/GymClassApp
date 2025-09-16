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
    public class GymClassServiceTests : IDisposable
    {
        private readonly DbContextOptions<GymContext> _dbContextOptions;
        private readonly Mock<ILogger<AgendamentoService>> _loggerMock;
        private GymContext _context;

        public GymClassServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<GymContext>()
                .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}")
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
        public async Task CriarAgendamentoAsync_DeveCriarAgendamento_QuandoValido()
        {
            var aluno = TestDataFactory.CriarAluno();
            var aula = TestDataFactory.CriarAula();

            _context.Alunos.Add(aluno);
            _context.Aulas.Add(aula);
            await _context.SaveChangesAsync();

            var service = new AgendamentoService(_context, _loggerMock.Object);
            var agendamentoDto = new AgendamentoCreateDto { AlunoId = 1, AulaId = 1 };

            var result = await service.CreateAgendamentoAsync(agendamentoDto);

            Assert.NotNull(result);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task CriarAgendamentoAsync_DeveLancarExcecao_QuandoAulaLotada()
        {
            var aluno = TestDataFactory.CriarAluno();
            var aula = TestDataFactory.CriarAula(capacidade: 1, vagasOcupadas: 1);

            _context.Alunos.Add(aluno);
            _context.Aulas.Add(aula);
            await _context.SaveChangesAsync();

            var service = new AgendamentoService(_context, _loggerMock.Object);
            var agendamentoDto = new AgendamentoCreateDto { AlunoId = 1, AulaId = 1 };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateAgendamentoAsync(agendamentoDto));
        }

        [Fact]
        public async Task CriarAgendamentoAsync_DeveLancarExcecao_QuandoLimitePlanoExcedido()
        {
            var aluno = TestDataFactory.CriarAluno(plano: PlanoType.Mensal);
            _context.Alunos.Add(aluno);
 
            for (int i = 1; i <= 13; i++)
            {
                var aula = TestDataFactory.CriarAula(id: i, tipo: $"Yoga {i}", capacidade: 20);
                _context.Aulas.Add(aula);
            }

            for (int i = 1; i <= 12; i++)
            {
                var agendamento = TestDataFactory.CriarAgendamento(
                    id: i,
                    alunoId: aluno.Id,
                    aulaId: i
                );
                _context.Agendamentos.Add(agendamento);

                var aula = await _context.Aulas.FindAsync(i);
                aula!.VagasOcupadas++;
            }

            await _context.SaveChangesAsync();

            var service = new AgendamentoService(_context, _loggerMock.Object);

            var agendamentoDto = new AgendamentoCreateDto { AlunoId = aluno.Id, AulaId = 13 };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateAgendamentoAsync(agendamentoDto));
        }
    }
}