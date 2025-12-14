namespace Stargate.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string username);
        string GenerateToken(string username, string role);
        bool ValidateToken(string token);
        string? GetUsernameFromToken(string token);
        void RevokeToken(string token);
    }
}
