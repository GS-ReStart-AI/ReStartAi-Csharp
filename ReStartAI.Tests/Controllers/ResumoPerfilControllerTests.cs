using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ReStartAI.Api.Controllers.Career;
using ReStartAI.Api.Integration;
using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;
using Xunit;

namespace ReStartAI.Tests.Controllers
{
    public class ResumoPerfilControllerTests
    {
        private readonly Mock<ICurriculoRepository> _curriculosMock;
        private readonly Mock<IResumeSummaryClient> _resumeSummaryMock;

        public ResumoPerfilControllerTests()
        {
            _curriculosMock = new Mock<ICurriculoRepository>();
            _resumeSummaryMock = new Mock<IResumeSummaryClient>();
        }

        private ResumoPerfilController CreateController()
        {
            return new ResumoPerfilController(_curriculosMock.Object, _resumeSummaryMock.Object);
        }

        [Fact]
        public async Task GetResumoPerfil_sem_usuarioId_deve_retornar_400()
        {
            var controller = CreateController();

            var result = await controller.GetResumoPerfil(usuarioId: null!, CancellationToken.None);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetResumoPerfil_sem_curriculo_para_usuario_deve_retornar_404()
        {
            var controller = CreateController();

            _curriculosMock
                .Setup(r => r.GetAllAsync(1, 50))
                .ReturnsAsync(new List<Curriculo>());

            var result = await controller.GetResumoPerfil("usuario-1", CancellationToken.None);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetResumoPerfil_com_curriculo_sem_texto_deve_retornar_503()
        {
            var controller = CreateController();

            var curriculos = new List<Curriculo>
            {
                new Curriculo
                {
                    Id = "c1",
                    UsuarioId = "usuario-1",
                    NomeArquivo = "cv.pdf",
                    Texto = "",
                    CriadoEm = DateTime.UtcNow.AddDays(-1)
                }
            };

            _curriculosMock
                .Setup(r => r.GetAllAsync(1, 50))
                .ReturnsAsync(curriculos);

            var result = await controller.GetResumoPerfil("usuario-1", CancellationToken.None);

            var obj = result.Result.Should().BeOfType<ObjectResult>().Subject;
            obj.StatusCode.Should().Be(503);
        }

        [Fact]
        public async Task GetResumoPerfil_com_curriculo_e_ai_ok_deve_retornar_200()
        {
            var controller = CreateController();

            var curriculos = new List<Curriculo>
            {
                new Curriculo
                {
                    Id = "c1",
                    UsuarioId = "usuario-1",
                    NomeArquivo = "cv.pdf",
                    Texto = "curriculo de teste com dados e atendimento ao cliente",
                    Skills = new List<string> { "atendimento", "comunicacao" },
                    CriadoEm = DateTime.UtcNow.AddYears(-1)
                }
            };

            _curriculosMock
                .Setup(r => r.GetAllAsync(1, 50))
                .ReturnsAsync(curriculos);

            _resumeSummaryMock
                .Setup(c => c.GenerateAsync("usuario-1", It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResumeSummaryResult(
                    Areas: new List<string> { "Atendimento ao Cliente", "Hospitalidade" },
                    BestRole: "Atendente de Cafeteria Jr",
                    Roles: new List<string> { "Atendente de Cafeteria Jr", "Auxiliar de Atendimento" },
                    Seniority: "junior",
                    YearsOfExperience: 1,
                    SkillsDetected: new List<string> { "Atendimento ao cliente", "Comunicação" }
                ));

            var result = await controller.GetResumoPerfil("usuario-1", CancellationToken.None);

            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var resumo = ok.Value.Should().BeOfType<ResumoPerfilController.ResumoResponse>().Subject;

            resumo.Areas.Should().NotBeNullOrEmpty();
            resumo.Areas.Should().Contain("Atendimento ao Cliente");
            resumo.Roles.Should().NotBeNullOrEmpty();
            resumo.Roles.Should().Contain("Atendente de Cafeteria Jr");
            resumo.Experiencias.Should().Be(1);
        }

        [Fact]
        public async Task GetResumoPerfil_com_curriculo_e_ai_falhando_deve_retornar_503()
        {
            var controller = CreateController();

            var curriculos = new List<Curriculo>
            {
                new Curriculo
                {
                    Id = "c1",
                    UsuarioId = "usuario-1",
                    NomeArquivo = "cv.pdf",
                    Texto = "curriculo de teste",
                    CriadoEm = DateTime.UtcNow.AddYears(-1)
                }
            };

            _curriculosMock
                .Setup(r => r.GetAllAsync(1, 50))
                .ReturnsAsync(curriculos);

            _resumeSummaryMock
                .Setup(c => c.GenerateAsync("usuario-1", It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("AI error"));

            var result = await controller.GetResumoPerfil("usuario-1", CancellationToken.None);

            var obj = result.Result.Should().BeOfType<ObjectResult>().Subject;
            obj.StatusCode.Should().Be(503);
        }
    }
}
