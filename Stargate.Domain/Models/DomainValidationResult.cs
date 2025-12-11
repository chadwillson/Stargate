namespace Stargate.Domain.Models
{
    public class DomainValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;

        public static DomainValidationResult Success()
        {
            return new DomainValidationResult
            {
                IsValid = true
            };
        }

        public static DomainValidationResult Failure(string errorMessage, string errorCode = "VALIDATION_ERROR")
        {
            return new DomainValidationResult
            {
                IsValid = false,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode
            };
        }
    }
}
