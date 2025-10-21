using Microsoft.AspNetCore.Http;
using SafeScribeAPI.Services;
using System.IdentityModel.Tokens.Jwt;

namespace SafeScribeApi.Middleware
{
    /// <summary>
    /// Middleware responsável por bloquear requisições autenticadas cujo token esteja na blacklist.
    /// Deve ser adicionado ao pipeline logo após app.UseAuthentication().
    /// </summary>
    public class JwtBlacklistMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtBlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITokenBlacklistService blacklistService)
        {
            // Verifica se a requisição está autenticada
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                // Extrai a claim "jti" do token JWT
                var jti = context.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

                if (!string.IsNullOrEmpty(jti))
                {
                    // Verifica se o token foi invalidado (logout)
                    var isBlacklisted = await blacklistService.IsBlacklistedAsync(jti);
                    if (isBlacklisted)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(new { error = "Token inválido. Faça login novamente." });
                        return;
                    }
                }
            }

            await _next(context); // Continua a pipeline
        }
    }
}
