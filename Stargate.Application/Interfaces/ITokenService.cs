namespace Stargate.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string username);
        bool ValidateToken(string token);
        string? GetUsernameFromToken(string token);
        void RevokeToken(string token);
    }
}
