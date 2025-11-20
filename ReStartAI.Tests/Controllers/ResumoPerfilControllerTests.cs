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
            return new ResumoPerfilController(
                _curriculosMock.Object,
                _resumeSummaryMock.Object,
                null!,
                null!,
                null!
            );
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetResumoPerfil_sem_usuarioId_deve_retornar_400(string? usuarioId)
        {
            var controller = CreateController();

            var result = await controller.GetResumoPerfil(usuarioId!, CancellationToken.None);

            var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("usuarioId obrigatório.");
        }

        [Fact]
        public async Task GetResumoPerfil_sem_curriculo_para_usuario_deve_retornar_404()
        {
            var controller = CreateController();

            var curriculos = new List<Curriculo>
            {
                new Curriculo
                {
                    Id = "c2",
                    UsuarioId = "outro-usuario",
                    NomeArquivo = "cv2.pdf",
                    Texto = "texto qualquer",
                    CriadoEm = DateTime.UtcNow.AddDays(-2)
                }
            };

            _curriculosMock
                .Setup(r => r.GetAllAsync(1, 50))
                .ReturnsAsync(curriculos);

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
            obj.Value.Should().Be("Currículo ainda não processado.");
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
                    SkillsDetected: Array.Empty<string>() // <- só isso muda
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
        public async Task GetResumoPerfil_quando_ai_retorna_sem_areas_e_roles_deve_usar_valores_padrao()
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
                    CriadoEm = DateTime.UtcNow
                }
            };

            _curriculosMock
                .Setup(r => r.GetAllAsync(1, 50))
                .ReturnsAsync(curriculos);

            _resumeSummaryMock
                .Setup(c => c.GenerateAsync("usuario-1", It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResumeSummaryResult(
                    Areas: Array.Empty<string>(),
                    BestRole: "",
                    Roles: Array.Empty<string>(),
                    Seniority: "",
                    YearsOfExperience: -1,
                    SkillsDetected: Array.Empty<string>()
                ));

            var result = await controller.GetResumoPerfil("usuario-1", CancellationToken.None);

            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var resumo = ok.Value.Should().BeOfType<ResumoPerfilController.ResumoResponse>().Subject;

            resumo.Areas.Should().ContainSingle().Which.Should().Be("Área em análise");
            resumo.Roles.Should().ContainSingle().Which.Should().Be("Papel em análise");
            resumo.Experiencias.Should().Be(0);
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
            obj.Value.Should().Be("Não foi possível gerar o resumo no momento.");
        }
    }
}
