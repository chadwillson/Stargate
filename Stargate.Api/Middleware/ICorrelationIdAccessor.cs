namespace Stargate.Api.Middleware
{
    public interface ICorrelationIdAccessor
    {
        string? CorrelationId { get; set; }
    }

    public class CorrelationIdAccessor : ICorrelationIdAccessor
    {
        public string? CorrelationId { get; set; }
    }
}
