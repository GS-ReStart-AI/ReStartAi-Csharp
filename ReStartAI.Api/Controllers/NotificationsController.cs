using Microsoft.AspNetCore.Mvc;
using ReStartAI.Api.Security;
using ReStartAI.Api.Swagger.Examples.Notificacao;
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
    public class NotificacaoController : ControllerBase
    {
        private readonly NotificacaoService _service;

        public NotificacaoController(NotificacaoService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista paginada de notificações",
            Description = "Retorna uma lista paginada de notificações com links HATEOAS."
        )]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(NotificacaoListResponseExample))]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync(page, pageSize);
            var total = await _service.CountAsync();

            var result = new PagedResult<object>(
                items.Select(i => HateoasHelper.WithLinks(i, "/api/v1/notificacao", i.Id)),
                total,
                page,
                pageSize
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Busca notificação por ID",
            Description = "Retorna os detalhes de uma notificação específica com links HATEOAS."
        )]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(NotificacaoResponseExample))]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var result = HateoasHelper.WithLinks(entity, "/api/v1/notificacao", id);
            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Cria uma nova notificação",
            Description = "Cria uma nova notificação para um usuário."
        )]
        [SwaggerRequestExample(typeof(Notificacao), typeof(NotificacaoRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(NotificacaoResponseExample))]
        public async Task<IActionResult> Create([FromBody] Notificacao entity)
        {
            var created = await _service.CreateAsync(entity);
            var result = HateoasHelper.WithLinks(created, "/api/v1/notificacao", created.Id);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Atualiza uma notificação",
            Description = "Atualiza completamente os dados de uma notificação existente."
        )]
        [SwaggerRequestExample(typeof(Notificacao), typeof(NotificacaoUpdateRequestExample))]
        public async Task<IActionResult> Update(string id, [FromBody] Notificacao entity)
        {
            await _service.UpdateAsync(id, entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Remove uma notificação",
            Description = "Remove permanentemente uma notificação."
        )]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(NotificacaoDeleteResponseExample))]
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            await _service.DeleteAsync(id);
            return Ok(new { message = "Notificação removida com sucesso" });
        }
    }
}
