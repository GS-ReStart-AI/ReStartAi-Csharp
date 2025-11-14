using Microsoft.AspNetCore.Mvc;
using ReStartAI.Api.Models.Usuarios;
using ReStartAI.Api.Swagger.Examples.Usuarios;
using ReStartAI.Application.Services;
using ReStartAI.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace ReStartAI.Api.Controllers
{
    [ApiController]
    [Route("api/v1/usuarios")]
    [Produces("application/json")]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioService _service;

        public UsuariosController(UsuarioService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista todos os usuários", Description = "Retorna a lista completa de usuários cadastrados.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UsuarioResponseListExample))]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var list = await _service.GetAllAsync(page, pageSize);
            return Ok(list.Select(MapToResponse));
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Busca usuário por ID", Description = "Retorna os dados completos de um usuário.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UsuarioResponseExample))]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            return Ok(MapToResponse(entity));
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cria um novo usuário", Description = "Cria um novo usuário no sistema.")]
        [SwaggerRequestExample(typeof(UsuarioCreateRequest), typeof(UsuarioCreateRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(UsuarioResponseExample))]
        public async Task<IActionResult> Create([FromBody] UsuarioCreateRequest request)
        {
            var entity = MapToEntity(request);
            var created = await _service.CreateAsync(entity);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToResponse(created));
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza um usuário", Description = "Atualiza os dados de um usuário existente.")]
        [SwaggerRequestExample(typeof(UsuarioCreateRequest), typeof(UsuarioUpdateRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UsuarioResponseExample))]
        public async Task<IActionResult> Update(string id, [FromBody] UsuarioCreateRequest request)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            entity.NomeCompleto = request.NomeCompleto;
            entity.Cpf = request.Cpf;
            entity.DataNascimento = request.DataNascimento;
            entity.Email = request.Email;

            await _service.UpdateAsync(id, entity);
            return Ok(MapToResponse(entity));
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Remove usuário", Description = "Exclui um usuário definitivamente.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UsuarioDeleteResponseExample))]
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            await _service.DeleteAsync(id);
            return Ok(new { message = "Usuário removido com sucesso" });
        }

        private static UsuarioResponse MapToResponse(Usuario e)
        {
            return new UsuarioResponse
            {
                UsuarioId = 0,
                NomeCompleto = e.NomeCompleto,
                Cpf = e.Cpf,
                DataNascimento = e.DataNascimento,
                Email = e.Email
            };
        }

        private static Usuario MapToEntity(UsuarioCreateRequest r)
        {
            return new Usuario
            {
                NomeCompleto = r.NomeCompleto,
                Cpf = r.Cpf,
                DataNascimento = r.DataNascimento,
                Email = r.Email
            };
        }
    }
}
