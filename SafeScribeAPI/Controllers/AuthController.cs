using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeScribeAPI.DTOs;
using SafeScribeAPI.Services;
using System.IdentityModel.Tokens.Jwt;

namespace SafeScribeAPI.Controllers
{
    /// <summary>
    /// Controlador responsável por autenticação e gerenciamento de sessão de usuários.
    /// 
    /// Contém endpoints para:
    /// - Registrar um novo usuário na plataforma
    /// - Realizar login e obter um token JWT
    /// - Efetuar logout, invalidando o token atual
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ITokenBlacklistService _blacklist;

        public AuthController(ITokenService tokenService, ITokenBlacklistService blacklist)
        {
            _tokenService = tokenService;
            _blacklist = blacklist;
        }

        /// <summary>
        /// 📥 Registra um novo usuário no sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint cria um novo usuário com a senha armazenada de forma segura (hash com BCrypt).
        /// 
        /// **Regras:**
        /// - Não requer autenticação.
        /// - O nome de usuário precisa ser único.
        /// - A role define o nível de acesso (ex: Admin, Editor, Leitor).
        /// 
        /// **Exemplo de requisição:**
        /// ```json
        /// {
        ///   "username": "joao",
        ///   "password": "SenhaSegura123",
        ///   "role": 2
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">Dados de cadastro do usuário.</param>
        /// <response code="200">Usuário criado com sucesso.</response>
        /// <response code="400">Nome de usuário já existe.</response>
        [HttpPost("registrar")]
        [AllowAnonymous]
        public async Task<IActionResult> Registrar([FromBody] UserRegisterDto dto)
        {
            var user = await _tokenService.RegisterAsync(dto.Username, dto.Password, dto.Role);
            return Ok(new
            {
                message = "✅ Usuário registrado com sucesso.",
                user = new { user.Id, user.Username, user.Role }
            });
        }

        /// <summary>
        /// 🔑 Realiza login e retorna um token JWT.
        /// </summary>
        /// <remarks>
        /// Este endpoint autentica o usuário com base em **username** e **password**.
        /// 
        /// Se as credenciais forem válidas, será retornado um **token JWT**, que deve ser utilizado no cabeçalho
        /// de autenticação (`Authorization: Bearer {token}`) para acessar rotas protegidas.
        /// 
        /// **Exemplo de requisição:**
        /// ```json
        /// {
        ///   "username": "joao",
        ///   "password": "SenhaSegura123"
        /// }
        /// ```
        /// 
        /// **Exemplo de resposta:**
        /// ```json
        /// {
        ///   "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///   "expiresAtUtc": "2025-10-21T22:33:00Z",
        ///   "username": "joao",
        ///   "role": "Editor"
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">Credenciais de login.</param>
        /// <response code="200">Login bem-sucedido e token retornado.</response>
        /// <response code="401">Credenciais inválidas.</response>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var result = await _tokenService.LoginAsync(dto.Username, dto.Password);

            // 🔒 Se usuário não existir ou senha for inválida
            if (result.user == null)
                return Unauthorized(new { error = "Credenciais inválidas." });

            var (user, token, expiresAtUtc) = result;

            return Ok(new TokenResponseDto
            {
                Token = token,
                ExpiresAtUtc = expiresAtUtc,
                Username = user.Username,
                Role = user.Role.ToString()
            });
        }

        /// <summary>
        /// 🚪 Realiza logout do usuário e invalida o token atual.
        /// </summary>
        /// <remarks>
        /// Este endpoint adiciona o token atual à **blacklist**, impedindo seu uso futuro.
        /// 
        /// **Regras:**
        /// - É necessário estar autenticado.
        /// - Após o logout, o token atual deixará de ser aceito em qualquer rota protegida.
        /// </remarks>
        /// <response code="200">Logout realizado com sucesso e token invalidado.</response>
        /// <response code="400">Token inválido.</response>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var jti = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (jti != null)
            {
                await _blacklist.AddToBlacklistAsync(jti);
                return Ok(new { message = "✅ Logout realizado com sucesso. Token invalidado." });
            }

            return BadRequest(new { error = "Token inválido." });
        }
    }
}