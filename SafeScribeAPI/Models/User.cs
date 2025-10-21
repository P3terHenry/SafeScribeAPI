using System.ComponentModel.DataAnnotations;

namespace SafeScribeAPI.Models
{
    /// <summary>
    /// Representa um usuário do sistema SafeScribe.
    /// </summary>
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MinLength(3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required]
        public Role Role { get; set; }
    }
}