using Microsoft.AspNetCore.Mvc;
using ReStartAI.Api.Swagger.Candidatura;
using ReStartAI.Application.Dto;
using ReStartAI.Application.Helpers;
using ReStartAI.Application.Services;
using ReStartAI.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace ReStartAI.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class CandidaturaController : ControllerBase
    {
        private readonly CandidaturaService _service;

        public CandidaturaController(CandidaturaService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista paginada de candidaturas",
            Description = "Retorna uma lista paginada de candidaturas com links HATEOAS."
        )]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CandidaturaListResponseExample))]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync(page, pageSize);
            var total = await _service.CountAsync();

            var result = new PagedResult<object>(
                items.Select(i => HateoasHelper.WithLinks(i, "/api/v1/candidatura", i.Id)),
                total,
                page,
                pageSize
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Busca candidatura por ID",
            Description = "Retorna os detalhes de uma candidatura específica com links HATEOAS."
        )]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CandidaturaResponseExample))]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var result = HateoasHelper.WithLinks(entity, "/api/v1/candidatura", id);
            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Cria uma nova candidatura",
            Description = "Cria uma nova candidatura vinculando usuário e vaga."
        )]
        [SwaggerRequestExample(typeof(Candidatura), typeof(CandidaturaRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(CandidaturaResponseExample))]
        public async Task<IActionResult> Create([FromBody] Candidatura entity)
        {
            var created = await _service.CreateAsync(entity);
            var result = HateoasHelper.WithLinks(created, "/api/v1/candidatura", created.Id);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Atualiza uma candidatura",
            Description = "Atualiza completamente os dados de uma candidatura existente."
        )]
        [SwaggerRequestExample(typeof(Candidatura), typeof(CandidaturaUpdateRequestExample))]
        public async Task<IActionResult> Update(string id, [FromBody] Candidatura entity)
        {
            await _service.UpdateAsync(id, entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Remove uma candidatura",
            Description = "Remove permanentemente uma candidatura."
        )]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CandidaturaDeleteResponseExample))]
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            await _service.DeleteAsync(id);
            return Ok(new { message = "Candidatura removida com sucesso" });
        }
    }
}
