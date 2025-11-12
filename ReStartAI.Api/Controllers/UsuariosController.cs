using Microsoft.AspNetCore.Mvc;
using ReStartAI.Domain.Entities;
using ReStartAI.Application.Services;
using ReStartAI.Infrastructure.Repositories;

namespace ReStartAI.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _service;
        private readonly LogRepository _log;

        public UsuarioController(UsuarioService service, LogRepository log)
        {
            _service = service;
            _log = log;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var result = await _service.GetAllAsync(page, pageSize);
            await _log.AddAsync($"Listagem de usuários retornada ({result.Count()} registros).");
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result is null)
            {
                await _log.AddAsync($"Usuário não encontrado: {id}");
                return NotFound();
            }

            await _log.AddAsync($"Usuário recuperado: {id}");
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Usuario entity)
        {
            var result = await _service.CreateAsync(entity);
            await _log.AddAsync($"Novo usuário criado: {result.Email} (ID: {result.Id})");
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Usuario entity)
        {
            await _service.UpdateAsync(id, entity);
            await _log.AddAsync($"Usuário atualizado: {id}");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeleteAsync(id);
            await _log.AddAsync($"Usuário excluído: {id}");
            return NoContent();
        }
    }
}
