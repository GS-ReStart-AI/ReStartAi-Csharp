using ReStartAI.Api.Models.Usuarios;
using Swashbuckle.AspNetCore.Filters;

namespace ReStartAI.Api.Swagger.Examples.Usuarios
{
    public class UsuarioCreateRequestExample : IExamplesProvider<UsuarioCreateRequest>
    {
        public UsuarioCreateRequest GetExamples()
        {
            return new UsuarioCreateRequest
            {
                NomeCompleto = "João da Silva",
                Cpf = "12345678901",
                DataNascimento = new DateTime(1995, 3, 15),
                Email = "joao.silva@example.com",
                Senha = "SenhaForte123!"
            };
        }
    }

    public class UsuarioResponseExample : IExamplesProvider<UsuarioResponse>
    {
        public UsuarioResponse GetExamples()
        {
            return new UsuarioResponse
            {
                UsuarioId = "000000000000000000000001",
                NomeCompleto = "João da Silva",
                Cpf = "12345678901",
                DataNascimento = new DateTime(1995, 3, 15),
                Email = "joao.silva@example.com"
            };
        }
    }

    public class UsuarioResponseListExample : IExamplesProvider<IEnumerable<UsuarioResponse>>
    {
        public IEnumerable<UsuarioResponse> GetExamples()
        {
            return new List<UsuarioResponse>
            {
                new UsuarioResponse
                {
                    UsuarioId = "000000000000000000000001",
                    NomeCompleto = "João da Silva",
                    Cpf = "12345678901",
                    DataNascimento = new DateTime(1995, 3, 15),
                    Email = "joao.silva@example.com"
                },
                new UsuarioResponse
                {
                    UsuarioId = "000000000000000000000002",
                    NomeCompleto = "Maria Oliveira",
                    Cpf = "98765432100",
                    DataNascimento = new DateTime(1990, 8, 22),
                    Email = "maria.oliveira@example.com"
                }
            };
        }
    }

    public class UsuarioUpdateRequestExample : IExamplesProvider<UsuarioCreateRequest>
    {
        public UsuarioCreateRequest GetExamples()
        {
            return new UsuarioCreateRequest
            {
                NomeCompleto = "João da Silva Atualizado",
                Cpf = "12345678901",
                DataNascimento = new DateTime(1995, 3, 15),
                Email = "joao.silva.atualizado@example.com",
                Senha = "NovaSenha123!"
            };
        }
    }

    public class UsuarioDeleteResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                message = "Usuário removido com sucesso"
            };
        }
    }
}
