using Microsoft.AspNetCore.Mvc;
using ReStartAI.Api.Security;
using ReStartAI.Api.Swagger.Examples.Curriculo;
using ReStartAI.Application.Dto;
using ReStartAI.Application.Helpers;
using ReStartAI.Application.Services;
using ReStartAI.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace ReStartAI.Api.Controllers
{
    [ApiController]
    [ApiKeyAuth] 
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class CurriculoController : ControllerBase
    {
        private readonly CurriculoService _service;

        public CurriculoController(CurriculoService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista paginada de currículos", Description = "Retorna uma lista paginada de currículos com links HATEOAS.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CurriculoListResponseExample))]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync(page, pageSize);
            var total = await _service.CountAsync();

            var result = new PagedResult<object>(
                items.Select(i => HateoasHelper.WithLinks(i, "/api/v1/curriculo", i.Id)),
                total,
                page,
                pageSize
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Busca currículo por ID", Description = "Retorna os detalhes de um currículo específico com links HATEOAS.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CurriculoResponseExample))]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var result = HateoasHelper.WithLinks(entity, "/api/v1/curriculo", id);
            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cria um novo currículo", Description = "Cria um novo currículo vinculado a um usuário.")]
        [SwaggerRequestExample(typeof(Curriculo), typeof(CurriculoRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(CurriculoResponseExample))]
        public async Task<IActionResult> Create([FromBody] Curriculo entity)
        {
            var created = await _service.CreateAsync(entity);
            var result = HateoasHelper.WithLinks(created, "/api/v1/curriculo", created.Id);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza um currículo", Description = "Atualiza completamente os dados de um currículo existente.")]
        [SwaggerRequestExample(typeof(Curriculo), typeof(CurriculoUpdateRequestExample))]
        public async Task<IActionResult> Update(string id, [FromBody] Curriculo entity)
        {
            await _service.UpdateAsync(id, entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Remove um currículo", Description = "Remove permanentemente um currículo.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CurriculoDeleteResponseExample))]
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            await _service.DeleteAsync(id);
            return Ok(new { message = "Currículo removido com sucesso" });
        }
    }
}
