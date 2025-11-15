using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ReStartAI.Api.Controllers.Career;
using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;
using Xunit;

namespace ReStartAI.Tests.Controllers
{
    public class ResumoPerfilControllerTests
    {
        private readonly Mock<ICurriculoRepository> _curriculoRepositoryMock;

        public ResumoPerfilControllerTests()
        {
            _curriculoRepositoryMock = new Mock<ICurriculoRepository>();
        }

        private ResumoPerfilController CreateControllerWithUser(string? uid)
        {
            var controller = new ResumoPerfilController(_curriculoRepositoryMock.Object);

            var claims = new List<Claim>();
            if (!string.IsNullOrEmpty(uid))
                claims.Add(new Claim("uid", uid));

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext { User = principal };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            return controller;
        }

        [Fact]
        public async Task GetResumoPerfil_sem_uid_deve_retornar_401()
        {
            var controller = CreateControllerWithUser(null);

            var result = await controller.GetResumoPerfil();

            result.Result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task GetResumoPerfil_sem_curriculo_deve_retornar_404()
        {
            var uid = "507f191e810c19729de860ea";
            var controller = CreateControllerWithUser(uid);

            _curriculoRepositoryMock
                .Setup(r => r.GetAllAsync(1, 10))
                .ReturnsAsync(new List<Curriculo>());

            var result = await controller.GetResumoPerfil();

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetResumoPerfil_com_curriculo_valido_deve_retornar_resumo_ok()
        {
            var uid = "507f191e810c19729de860ea";
            var controller = CreateControllerWithUser(uid);

            var curriculo = new Curriculo
            {
                Id = "507f191e810c19729de860eb",
                UsuarioId = uid,
                Skills = new List<string> { "C#", "Backend", "APIs REST" },
                CriadoEm = DateTime.UtcNow.AddYears(-3)
            };

            _curriculoRepositoryMock
                .Setup(r => r.GetAllAsync(1, 10))
                .ReturnsAsync(new List<Curriculo> { curriculo });

            var result = await controller.GetResumoPerfil();

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();

            var resumo = okResult!.Value as ResumoPerfilController.ResumoResponse;
            resumo.Should().NotBeNull();

            resumo!.Areas.Should().Contain("Back-end .NET");
            resumo.Areas.Should().Contain("APIs e Integrações");
            resumo.Experiencias.Should().BeGreaterThanOrEqualTo(1);
        }
    }
}
