using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SafeScribeAPI.Data;
using SafeScribeAPI.Models;
using System.Security.Claims;
using System.Text;

namespace SafeScribeAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public TokenService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // Registro de usuário com hash da senha
        public async Task<User> RegisterAsync(string username, string password, Role role)
        {
            if (await _db.Users.AnyAsync(u => u.Username == username))
                throw new InvalidOperationException("Nome de usuário já existe.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        // Login e geração do token JWT
        public async Task<(User user, string token, DateTime expiresAtUtc)> LoginAsync(string username, string password)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                // ⚠️ Use default! para ignorar a checagem de nullability
                return (default!, default!, DateTime.MinValue);
            }

            var token = GenerateToken(user, out var expiresAtUtc);
            return (user, token, expiresAtUtc);
        }




        // Geração do JWT com claims essenciais
        public string GenerateToken(User user, out DateTime expiresAtUtc)
        {
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var secret = _config["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret não configurado.");
            var expiresMinutes = int.TryParse(_config["Jwt:ExpiresMinutes"], out var mins) ? mins : 60;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: credentials
            );

            expiresAtUtc = token.ValidTo;
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
