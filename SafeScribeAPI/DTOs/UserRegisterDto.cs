using SafeScribeAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace SafeScribeAPI.DTOs
{
    public class UserRegisterDto
    {
        /// <summary>
        /// Nome de usuário único para login.
        /// </summary>
        /// <example>joao.silva</example>
        [Required]
        [MinLength(3)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Senha do usuário (será criptografada no backend).
        /// </summary>
        /// <example>SenhaForte123</example>
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Nível de acesso do usuário.
        /// </summary>
        /// <example>Editor</example>
        [Required]
        public Role Role { get; set; }
    }
}
