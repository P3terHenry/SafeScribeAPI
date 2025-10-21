using System.Runtime.Serialization;

namespace SafeScribeAPI.Models
{
    /// <summary>
    /// Define os níveis de acesso dos usuários no sistema.
    /// </summary>
    public enum Role
    {
        /// <summary>
        /// Pode apenas visualizar as notas autorizadas.
        /// </summary>
        [EnumMember(Value = "Leitor")]
        Leitor = 0,
        
        /// <summary>
        /// Pode criar e editar suas próprias notas.
        /// </summary>
        [EnumMember(Value = "Editor de Notas")]
        Editor = 1,
        
        /// <summary>
        /// Acesso total ao sistema, incluindo gerenciamento de usuários e notas.
        /// </summary>
        [EnumMember(Value = "Administrador")]
        Admin = 2
    }
}
