using Microsoft.AspNetCore.Mvc;
using ReStartAI.Application.Dto;
using ReStartAI.Application.Helpers;
using ReStartAI.Application.Services;
using ReStartAI.Domain.Entities;

namespace ReStartAI.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CurriculoController : ControllerBase
    {
        private readonly CurriculoService _service;

        public CurriculoController(CurriculoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync(page, pageSize);
            var total = await _service.CountAsync();
            var result = new PagedResult<object>(
                items.Select(i => HateoasHelper.WithLinks(i, "/api/v1/curriculo", i.Id)),
                total, page, pageSize
            );
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();
            var result = HateoasHelper.WithLinks(entity, "/api/v1/curriculo", id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Curriculo entity)
        {
            var created = await _service.CreateAsync(entity);
            var result = HateoasHelper.WithLinks(created, "/api/v1/curriculo", created.Id);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Curriculo entity)
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
