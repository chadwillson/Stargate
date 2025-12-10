using System.Collections.Concurrent;
using Stargate.Application.Interfaces;

namespace Stargate.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly ConcurrentDictionary<string, string> _tokens = new();

        public string GenerateToken(string username)
        {
            var token = Guid.NewGuid().ToString();
            _tokens[token] = username;
            return token;
        }

        public bool ValidateToken(string token)
        {
            return _tokens.ContainsKey(token);
        }

        public string? GetUsernameFromToken(string token)
        {
            return _tokens.TryGetValue(token, out var username) ? username : null;
        }

        public void RevokeToken(string token)
        {
            _tokens.TryRemove(token, out _);
        }
    }
}
