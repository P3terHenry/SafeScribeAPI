using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeScribeAPI.Services;

namespace SafeScribeAPI.Controllers
{
    /// <summary>
    /// Controlador responsável por visualizar os tokens inválidos (blacklist).
    /// 
    /// Essa rota é útil para fins de debug, auditoria e avaliação: mostra todos os tokens que já foram invalidados via logout.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "Admin")] // 🔐 somente administradores podem consultar a blacklist
    public class BlacklistController : ControllerBase
    {
        private readonly ITokenBlacklistService _blacklistService;

        public BlacklistController(ITokenBlacklistService blacklistService)
        {
            _blacklistService = blacklistService;
        }

        /// <summary>
        /// 📜 Retorna todos os tokens que já foram adicionados à blacklist.
        /// </summary>
        /// <remarks>
        /// **Regras de acesso:**  
        /// - Somente usuários com **Role = Admin** podem acessar essa rota.  
        /// 
        /// **Sobre o retorno:**  
        /// - Os valores exibidos no campo `tokens` são os **identificadores únicos dos tokens (`jti`)**.  
        /// - Cada `jti` é gerado no momento do login e representa **um token específico**.  
        /// - Após o logout, o token associado a esse `jti` não pode mais ser usado.
        /// 
        /// **Exemplo de resposta:**
        /// ```json
        /// {
        ///   "message": "🛑 Tokens atualmente na blacklist.",
        ///   "count": 1,
        ///   "tokens": [
        ///     "a879bc8e-4a34-4b0f-8a02-1e2b95d9d8ad"
        ///   ]
        /// }
        /// ```
        /// </remarks>
        /// <response code="200">Lista de tokens na blacklist retornada com sucesso.</response>
        /// <response code="403">Acesso negado: apenas administradores podem acessar esta rota.</response>
        [HttpGet]
        public async Task<IActionResult> ListarBlacklist()
        {
            var tokens = await _blacklistService.GetAllBlacklistedTokensAsync();

            return Ok(new
            {
                message = "🛑 Tokens atualmente na blacklist.",
                count = tokens.Count,
                tokens
            });
        }
    }
}
