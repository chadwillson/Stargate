using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stargate.Domain.Dtos;

namespace Stargate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class UILoggingController : ControllerBase
    {
        private readonly ILogger<UILoggingController> _logger;

        public UILoggingController(ILogger<UILoggingController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Receives logs from the UI and writes them to the server log file
        /// </summary>
        [HttpPost]
        public IActionResult LogFromUI([FromBody] UILogRequest logRequest)
        {
            try
            {
                var logMessage = $"[UI] {logRequest.Message}";

                // Add additional context if available
                var additionalInfo = new List<string>();
                if (!string.IsNullOrEmpty(logRequest.Url))
                    additionalInfo.Add($"URL: {logRequest.Url}");
                if (!string.IsNullOrEmpty(logRequest.UserAgent))
                    additionalInfo.Add($"UserAgent: {logRequest.UserAgent}");
                if (!string.IsNullOrEmpty(logRequest.AdditionalData))
                    additionalInfo.Add($"Data: {logRequest.AdditionalData}");

                if (additionalInfo.Any())
                {
                    logMessage += " | " + string.Join(" | ", additionalInfo);
                }

                // Log based on level
                switch (logRequest.Level.ToLower())
                {
                    case "error":
                        if (!string.IsNullOrEmpty(logRequest.StackTrace))
                        {
                            LogUIErrorWithStackTrace(logMessage, logRequest.StackTrace);
                        }
                        else
                        {
                            LogUIError(logMessage);
                        }
                        break;
                    case "warning":
                    case "warn":
                        LogUIWarning(logMessage);
                        break;
                    case "info":
                    default:
                        LogUIInfo(logMessage);
                        break;
                }

                return Ok(new BaseResponse
                {
                    Success = true,
                    Message = "Log received"
                });
            }
            catch (Exception ex)
            {
                LogErrorProcessingUILog(ex);
                return StatusCode(500, new BaseResponse
                {
                    Success = false,
                    Message = "Failed to process log"
                });
            }
        }
    }
}
