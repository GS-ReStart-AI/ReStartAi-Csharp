using FluentAssertions;
using Moq;
using ReStartAI.Application.Services;
using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;
using Xunit;

namespace ReStartAI.Tests.Services
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _repositoryMock;
        private readonly UsuarioService _service;

        public UsuarioServiceTests()
        {
            _repositoryMock = new Mock<IUsuarioRepository>();
            _service = new UsuarioService(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_deve_retornar_usuario_quando_existir()
        {
            var id = "507f191e810c19729de860ea";

            var usuario = new Usuario
            {
                Id = id,
                NomeCompleto = "João da Silva",
                Email = "joao@example.com",
                Cpf = "12345678901"
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(usuario);

            var result = await _service.GetByIdAsync(id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
            _repositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_deve_chamar_repositorio_com_usuario()
        {
            var usuario = new Usuario
            {
                Id = "507f191e810c19729de860ea",
                NomeCompleto = "João da Silva",
                Email = "joao@example.com",
                Cpf = "12345678901"
            };

            _repositoryMock
                .Setup(r => r.CreateAsync(usuario))
                .ReturnsAsync(usuario);

            var result = await _service.CreateAsync(usuario);

            result.Should().BeSameAs(usuario);
            _repositoryMock.Verify(r => r.CreateAsync(usuario), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_deve_chamar_repositorio_com_id()
        {
            var id = "507f191e810c19729de860ea";

            _repositoryMock
                .Setup(r => r.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            await _service.DeleteAsync(id);

            _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }
    }
}
