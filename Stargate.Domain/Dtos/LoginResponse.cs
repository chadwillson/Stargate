namespace Stargate.Domain.Dtos
{
    public class LoginResponse : BaseResponse
    {
        public string? Token { get; set; }
        public string? Username { get; set; }
    }
}
