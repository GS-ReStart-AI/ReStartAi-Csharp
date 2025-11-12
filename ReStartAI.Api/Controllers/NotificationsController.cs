using Microsoft.AspNetCore.Mvc;
using ReStartAI.Domain.Entities;
using ReStartAI.Application.Services;

namespace ReStartAI.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class NotificacaoController : ControllerBase
    {
        private readonly NotificacaoService _service;

        public NotificacaoController(NotificacaoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var result = await _service.GetAllAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _service.GetByIdAsync(id);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Notificacao entity)
        {
            var result = await _service.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Notificacao entity)
        {
            await _service.UpdateAsync(id, entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}