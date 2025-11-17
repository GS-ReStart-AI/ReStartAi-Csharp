using Microsoft.AspNetCore.Mvc;
using ReStartAI.Api.Security;
using ReStartAI.Application.Helpers;
using ReStartAI.Application.Pdf;
using ReStartAI.Application.Services;
using ReStartAI.Domain.Entities;

namespace ReStartAI.Api.Controllers
{
    [ApiController]
    [ApiKeyAuth]
    [Route("api/v1/curriculo/upload")]
    [Produces("application/json")]
    public class CurriculoUploadController : ControllerBase
    {
        private readonly CurriculoService _service;
        private readonly IPdfTextExtractor _pdfTextExtractor;

        public CurriculoUploadController(CurriculoService service, IPdfTextExtractor pdfTextExtractor)
        {
            _service = service;
            _pdfTextExtractor = pdfTextExtractor;
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string usuarioId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Arquivo inválido.");

            if (string.IsNullOrWhiteSpace(usuarioId))
                return BadRequest("usuarioId obrigatório.");

            using var stream = file.OpenReadStream();
            var texto = _pdfTextExtractor.ExtractText(stream);

            var curriculo = new Curriculo
            {
                UsuarioId = usuarioId,
                NomeArquivo = file.FileName,
                Texto = texto,
                Skills = new List<string>()
            };

            var created = await _service.CreateAsync(curriculo);

            var resource = HateoasHelper.WithLinks(created, "/api/v1/curriculo", created.Id);
            return Created($"/api/v1/curriculo/{created.Id}", resource);
        }
    }
}