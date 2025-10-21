using System.Collections.Concurrent;

namespace SafeScribeAPI.Services
{
    public class InMemoryTokenBlacklistService : ITokenBlacklistService
    {
        // 🗑️ Dicionário: JTI -> Data de expiração
        private readonly ConcurrentDictionary<string, DateTime> _blacklist = new();

        public Task AddToBlacklistAsync(string jti, DateTime? expiresAtUtc = null)
        {
            var exp = expiresAtUtc ?? DateTime.UtcNow.AddHours(1);
            _blacklist[jti] = exp;
            return Task.CompletedTask;
        }

        public Task<bool> IsBlacklistedAsync(string jti)
        {
            if (_blacklist.TryGetValue(jti, out var exp))
            {
                if (DateTime.UtcNow <= exp)
                    return Task.FromResult(true);

                // 🧹 Remove automaticamente tokens expirados
                _blacklist.TryRemove(jti, out _);
            }

            return Task.FromResult(false);
        }

        public Task<List<string>> GetAllBlacklistedTokensAsync()
        {
            // ✅ Retorna apenas os tokens válidos (não expirados)
            var tokens = _blacklist
                .Where(kvp => kvp.Value > DateTime.UtcNow)
                .Select(kvp => kvp.Key)
                .ToList();

            return Task.FromResult(tokens);
        }
    }
}
