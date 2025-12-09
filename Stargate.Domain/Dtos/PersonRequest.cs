namespace Stargate.Domain.Dtos
{
    public class PersonRequest : PersonBaseRequest
    {
        public required string Name { get; set; } = string.Empty;
    }
}
