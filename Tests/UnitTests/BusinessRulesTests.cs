using GymClass.Models;
using GymClass.Services;
using GymClass.Tests.TestUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.UnitTests
{
    public class BusinessRulesTests : IDisposable
    {
        private readonly DbContextOptions<GymContext> _dbContextOptions;
        private readonly Mock<ILogger<GymClassService>> _loggerMock;
        private GymClassContext _context;

        public BusinessRulesTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<GymClassContext>()
                .UseInMemoryDatabase(databaseName: $"BusinessRulesTest_{Guid.NewGuid()}")
                .Options;

            _loggerMock = new Mock<ILogger<GymClassService>>();
            _context = new GymClassContext(_dbContextOptions);
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task Should_Allow_Agendamento_Within_Limit()
        {
            // Arrange
            var aluno = TestDataFactory.CreateAluno(plano: PlanoType.Mensal);
            var aula = TestDataFactory.CreateAula();

            _context.Alunos.Add(aluno);
            _context.Aulas.Add(aula);

            // Criar 11 agendamentos (1 abaixo do limite)
            for (int i = 0; i < 11; i++)
            {
                _context.Agendamentos.Add(TestDataFactory.CreateAgendamento(id: i + 1));
            }

            await _context.SaveChangesAsync();

            var service = new GymClassService(_context, _loggerMock.Object);
            var agendamentoDto = new AgendamentoCreateDto { AlunoId = 1, AulaId = 1 };

            // Act
            var result = await service.CreateAgendamentoAsync(agendamentoDto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task Should_Block_Agendamento_When_Duplicate()
        {
            // Arrange
            var aluno = TestDataFactory.CreateAluno();
            var aula = TestDataFactory.CreateAula();

            _context.Alunos.Add(aluno);
            _context.Aulas.Add(aula);
            _context.Agendamentos.Add(TestDataFactory.CreateAgendamento());

            await _context.SaveChangesAsync();

            var service = new GymClassService(_context, _loggerMock.Object);
            var agendamentoDto = new AgendamentoCreateDto { AlunoId = 1, AulaId = 1 };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateAgendamentoAsync(agendamentoDto));
        }

        [Fact]
        public async Task Should_Block_Agendamento_When_AulaInativa()
        {
            // Arrange
            var aluno = TestDataFactory.CreateAluno();
            var aula = TestDataFactory.CreateAula();
            aula.Ativa = false;

            _context.Alunos.Add(aluno);
            _context.Aulas.Add(aula);
            await _context.SaveChangesAsync();

            var service = new GymClassService(_context, _loggerMock.Object);
            var agendamentoDto = new AgendamentoCreateDto { AlunoId = 1, AulaId = 1 };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateAgendamentoAsync(agendamentoDto));
        }

        [Fact]
        public async Task Should_Block_Agendamento_When_AlunoInativo()
        {
            // Arrange
            var aluno = TestDataFactory.CreateAluno();
            aluno.Ativo = false;
            var aula = TestDataFactory.CreateAula();

            _context.Alunos.Add(aluno);
            _context.Aulas.Add(aula);
            await _context.SaveChangesAsync();

            var service = new GymClassService(_context, _loggerMock.Object);
            var agendamentoDto = new AgendamentoCreateDto { AlunoId = 1, AulaId = 1 };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateAgendamentoAsync(agendamentoDto));
        }
    }
}
