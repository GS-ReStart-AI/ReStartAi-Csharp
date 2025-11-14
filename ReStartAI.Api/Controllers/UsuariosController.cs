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
        [SwaggerOperation(
            Summary = "Lista todos os usuários",
            Description = "Retorna a lista paginada de usuários cadastrados."
        )]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UsuarioResponseListExample))]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var list = await _service.GetAllAsync(page, pageSize);
            var result = list.Select(MapToResponse);
            return Ok(result);
        }

        [HttpGet("{id:length(24)}")]
        [SwaggerOperation(
            Summary = "Busca usuário por ID",
            Description = "Retorna os dados completos de um usuário pelo seu ObjectId do MongoDB."
        )]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UsuarioResponseExample))]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
                return NotFound();

            return Ok(MapToResponse(entity));
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Cria um novo usuário",
            Description = "Cria um novo usuário no sistema."
        )]
        [SwaggerRequestExample(typeof(UsuarioCreateRequest), typeof(UsuarioCreateRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(UsuarioResponseExample))]
        public async Task<IActionResult> Create([FromBody] UsuarioCreateRequest request)
        {
            var entity = MapToEntity(request);
            var created = await _service.CreateAsync(entity);

            var response = MapToResponse(created);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
        }

        [HttpPut("{id:length(24)}")]
        [SwaggerOperation(
            Summary = "Atualiza um usuário",
            Description = "Atualiza os dados de um usuário existente pelo seu ObjectId do MongoDB."
        )]
        [SwaggerRequestExample(typeof(UsuarioCreateRequest), typeof(UsuarioUpdateRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UsuarioResponseExample))]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UsuarioCreateRequest request)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
                return NotFound();

            entity.NomeCompleto = request.NomeCompleto;
            entity.Cpf = request.Cpf;
            entity.DataNascimento = request.DataNascimento;
            entity.Email = request.Email;

            await _service.UpdateAsync(id, entity);

            return Ok(MapToResponse(entity));
        }

        [HttpDelete("{id:length(24)}")]
        [SwaggerOperation(
            Summary = "Remove usuário",
            Description = "Exclui um usuário definitivamente pelo seu ObjectId do MongoDB."
        )]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UsuarioDeleteResponseExample))]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
                return NotFound();

            await _service.DeleteAsync(id);
            return Ok(new { message = "Usuário removido com sucesso" });
        }

        private static UsuarioResponse MapToResponse(Usuario e)
        {
            return new UsuarioResponse
            {
                UsuarioId = e.Id,
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
