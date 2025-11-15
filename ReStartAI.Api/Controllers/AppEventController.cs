using Microsoft.AspNetCore.Mvc;
using ReStartAI.Api.Security;
using ReStartAI.Api.Swagger.Examples.AppEvent;
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
    public class AppEventController : ControllerBase
    {
        private readonly AppEventService _service;

        public AppEventController(AppEventService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista paginada de eventos do app",
            Description = "Retorna uma lista paginada de eventos de comportamento do usuário no app com links HATEOAS."
        )]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AppEventListResponseExample))]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync(page, pageSize);
            var total = await _service.CountAsync();

            var result = new PagedResult<object>(
                items.Select(i => HateoasHelper.WithLinks(i, "/api/v1/appevent", i.Id)),
                total,
                page,
                pageSize
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Busca evento do app por ID",
            Description = "Retorna os detalhes de um evento específico com links HATEOAS."
        )]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AppEventResponseExample))]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var result = HateoasHelper.WithLinks(entity, "/api/v1/appevent", id);
            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Registra um novo evento do app",
            Description = "Cria um novo evento de comportamento do usuário no app."
        )]
        [SwaggerRequestExample(typeof(AppEvent), typeof(AppEventRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(AppEventResponseExample))]
        public async Task<IActionResult> Create([FromBody] AppEvent entity)
        {
            var created = await _service.CreateAsync(entity);
            var result = HateoasHelper.WithLinks(created, "/api/v1/appevent", created.Id);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Atualiza um evento do app",
            Description = "Atualiza completamente os dados de um evento existente."
        )]
        [SwaggerRequestExample(typeof(AppEvent), typeof(AppEventUpdateRequestExample))]
        public async Task<IActionResult> Update(string id, [FromBody] AppEvent entity)
        {
            await _service.UpdateAsync(id, entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Remove um evento do app",
            Description = "Remove permanentemente um evento do app."
        )]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AppEventDeleteResponseExample))]
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            await _service.DeleteAsync(id);
            return Ok(new { message = "Evento de app removido com sucesso" });
        }
    }
}
