using Microsoft.AspNetCore.Mvc;
using ReStartAI.Api.Swagger.Examples.Vaga;
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
    public class VagaController : ControllerBase
    {
        private readonly VagaService _service;

        public VagaController(VagaService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista paginada de vagas", Description = "Retorna uma lista paginada de vagas com links HATEOAS.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(VagaListResponseExample))]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync(page, pageSize);
            var total = await _service.CountAsync();

            var result = new PagedResult<object>(
                items.Select(i => HateoasHelper.WithLinks(i, "/api/v1/vaga", i.Id)),
                total,
                page,
                pageSize
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Busca vaga por ID", Description = "Retorna os detalhes de uma vaga específica com links HATEOAS.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(VagaResponseExample))]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var result = HateoasHelper.WithLinks(entity, "/api/v1/vaga", id);
            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cria uma nova vaga", Description = "Cria uma nova vaga no sistema.")]
        [SwaggerRequestExample(typeof(Vaga), typeof(VagaRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(VagaResponseExample))]
        public async Task<IActionResult> Create([FromBody] Vaga entity)
        {
            var created = await _service.CreateAsync(entity);
            var result = HateoasHelper.WithLinks(created, "/api/v1/vaga", created.Id);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza uma vaga", Description = "Atualiza completamente os dados de uma vaga existente.")]
        [SwaggerRequestExample(typeof(Vaga), typeof(VagaUpdateRequestExample))]
        public async Task<IActionResult> Update(string id, [FromBody] Vaga entity)
        {
            await _service.UpdateAsync(id, entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Remove uma vaga", Description = "Exclui uma vaga definitivamente.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(VagaDeleteResponseExample))]
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            await _service.DeleteAsync(id);
            return Ok(new { message = "Vaga removida com sucesso" });
        }
    }
}
