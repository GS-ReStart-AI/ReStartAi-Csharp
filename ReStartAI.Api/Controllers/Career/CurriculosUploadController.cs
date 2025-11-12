using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReStartAI.Application.Parsing;
using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;
using UglyToad.PdfPig;

namespace ReStartAI.Api.Controllers.Career
{
    [ApiController]
    [Route("api/curriculos")]
    [Authorize]
    public class CurriculosUploadController : ControllerBase
    {
        private readonly ICurriculoRepository _curriculos;

        public CurriculosUploadController(ICurriculoRepository curriculos)
        {
            _curriculos = curriculos;
        }

        public record PostPdfResponse(string CurriculoId, string? Nome, string? Email, string? Telefone, List<string> Skills, List<string> Areas, List<string> PapeisSugeridos);

        [HttpPost("pdf")]
        [RequestSizeLimit(10_000_000)]
        public async Task<ActionResult<PostPdfResponse>> PostPdf([FromForm] IFormFile file)
        {
            var uid = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(uid)) return Unauthorized();
            if (file == null || file.Length == 0) return BadRequest();

            if (!string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase)
                && !file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest();

            string extracted;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                ms.Position = 0;
                using var doc = PdfDocument.Open(ms);
                var parts = new List<string>();
                foreach (var page in doc.GetPages())
                    parts.Add(page.Text);
                extracted = string.Join("\n", parts);
            }

            var texto = extracted.Length > 10_000 ? extracted[..10_000] : extracted;

            var parser = new ResumeParser();
            var parsed = parser.Parse(texto);

            var c = new Curriculo
            {
                UsuarioId = uid,
                NomeArquivo = file.FileName,
                Texto = texto,
                Skills = parsed.Skills,
                CriadoEm = DateTime.UtcNow
            };

            await _curriculos.CreateAsync(c);

            return Created($"api/curriculos/{c.Id}", new PostPdfResponse(c.Id!, parsed.Nome, parsed.Email, parsed.Telefone, parsed.Skills, parsed.Areas, parsed.PapeisSugeridos));
        }
    }
}
