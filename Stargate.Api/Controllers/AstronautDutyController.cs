using System.Diagnostics;
using System.Net;

using Microsoft.AspNetCore.Mvc;

using Stargate.Api.Middleware;
using Stargate.Application.Interfaces;
using Stargate.Domain.Dtos;

namespace Stargate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AstronautDutyController : ControllerBase
    {
        private readonly IAstronautDutyService _astronautDutyService;
        private readonly ILoggingService _loggingService;
        private readonly ICorrelationIdAccessor _correlationIdAccessor;
        private const string Category = "AstronautDutyController";

        public AstronautDutyController(IAstronautDutyService astronautDutyService, ILoggingService loggingService, ICorrelationIdAccessor correlationIdAccessor)
        {
            _astronautDutyService = astronautDutyService;
            _loggingService = loggingService;
            _correlationIdAccessor = correlationIdAccessor;
        }

        private string? CorrelationId => _correlationIdAccessor.CorrelationId;

        [HttpGet("{name}")]
        public async Task<IActionResult> GetAstronautDutiesByName(string name)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await _loggingService.LogInformationAsync(Category, $"GET /api/astronautduty/{name} - Retrieving astronaut duties", source: nameof(GetAstronautDutiesByName), correlationId: CorrelationId);

                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _astronautDutyService.GetAstronautDutiesByName(name, CorrelationId, cancellationToken);

                stopwatch.Stop();
                await _loggingService.LogRequestAsync($"/api/astronautduty/{name}", "GET", result.ResponseCode, stopwatch.ElapsedMilliseconds, correlationId: CorrelationId);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                await _loggingService.LogErrorAsync(Category, $"GET /api/astronautduty/{name} failed: {ex.Message}", ex, source: nameof(GetAstronautDutiesByName), correlationId: CorrelationId);
                await _loggingService.LogRequestAsync($"/api/astronautduty/{name}", "GET", 500, stopwatch.ElapsedMilliseconds, correlationId: CorrelationId);

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAstronautDuty([FromBody] CreateAstronautDutyResponse request)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await _loggingService.LogInformationAsync(Category, $"POST /api/astronautduty - Creating duty for: {request.Name}, Title: {request.DutyTitle}", source: nameof(CreateAstronautDuty), correlationId: CorrelationId);

                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _astronautDutyService.CreateAstronautDuty(request, CorrelationId, cancellationToken);

                stopwatch.Stop();
                await _loggingService.LogRequestAsync("/api/astronautduty", "POST", result.ResponseCode, stopwatch.ElapsedMilliseconds, correlationId: CorrelationId);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                await _loggingService.LogErrorAsync(Category, $"POST /api/astronautduty failed: {ex.Message}", ex, source: nameof(CreateAstronautDuty), correlationId: CorrelationId);
                await _loggingService.LogRequestAsync("/api/astronautduty", "POST", 500, stopwatch.ElapsedMilliseconds, correlationId: CorrelationId);

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }
    }
}
