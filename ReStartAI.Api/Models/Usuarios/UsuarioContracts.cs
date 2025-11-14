using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ReStartAI.Api.Models.Usuarios
{
    public sealed class UsuarioCreateRequest
    {
        [Required]
        [StringLength(120)]
        [DefaultValue("João da Silva")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required]
        [StringLength(11, MinimumLength = 11)]
        [DefaultValue("12345678901")]
        public string Cpf { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [DefaultValue("1995-03-15")]
        public DateTime DataNascimento { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        [DefaultValue("joao.silva@example.com")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        [DefaultValue("SenhaForte123!")]
        public string Senha { get; set; } = string.Empty;
    }

    public sealed class UsuarioResponse
    {
        [DefaultValue(1)]
        public long UsuarioId { get; set; }

        [DefaultValue("João da Silva")]
        public string NomeCompleto { get; set; } = string.Empty;

        [DefaultValue("12345678901")]
        public string Cpf { get; set; } = string.Empty;

        [DefaultValue("1995-03-15")]
        public DateTime DataNascimento { get; set; }

        [DefaultValue("joao.silva@example.com")]
        public string Email { get; set; } = string.Empty;
    }
}