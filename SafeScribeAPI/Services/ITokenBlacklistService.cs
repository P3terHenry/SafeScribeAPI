namespace SafeScribeAPI.Services
{
    public interface ITokenBlacklistService
    {
        Task AddToBlacklistAsync(string jti, DateTime? expiresAtUtc = null);
        Task<bool> IsBlacklistedAsync(string jti);

        Task<List<string>> GetAllBlacklistedTokensAsync();
    }
}
