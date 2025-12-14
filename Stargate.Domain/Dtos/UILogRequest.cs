namespace Stargate.Domain.Dtos
{
    public class UILogRequest
    {
        public string Level { get; set; } = "Info"; // Info, Warning, Error
        public string Message { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? UserAgent { get; set; }
        public string? StackTrace { get; set; }
        public string? AdditionalData { get; set; }
    }
}
