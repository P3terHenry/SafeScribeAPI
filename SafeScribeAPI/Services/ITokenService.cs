using SafeScribeAPI.Models;

namespace SafeScribeAPI.Services
{
    public interface ITokenService
    {
        Task<User> RegisterAsync(string username, string password, Role role);
        Task<(User user, string token, DateTime expiresAtUtc)> LoginAsync(string username, string password);
        string GenerateToken(User user, out DateTime expiresAtUtc);
    }
}
