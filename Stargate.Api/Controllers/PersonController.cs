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
    public class PersonController : ControllerBase
    {
        private readonly IPersonAstronautService _personAstronautService;
        private readonly ILoggingService _loggingService;
        private readonly ICorrelationIdAccessor _correlationIdAccessor;
        private const string Category = "PersonController";

        public PersonController(IPersonAstronautService personAstronautServic, ILoggingService loggingService, ICorrelationIdAccessor correlationIdAccessor)
        {
            _personAstronautService = personAstronautServic;
            _loggingService = loggingService;
            _correlationIdAccessor = correlationIdAccessor;
        }

        private string? CorrelationId => _correlationIdAccessor.CorrelationId;

        [HttpGet]
        public async Task<IActionResult> GetPeople()
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await _loggingService.LogInformationAsync(Category, "GET /api/person - Retrieving all people", source: nameof(GetPeople), correlationId: CorrelationId);

                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _personAstronautService.GetPeople(CorrelationId, cancellationToken);

                stopwatch.Stop();
                await _loggingService.LogRequestAsync("/api/person", "GET", result.ResponseCode, stopwatch.ElapsedMilliseconds, correlationId: CorrelationId);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                await _loggingService.LogErrorAsync(Category, $"GET /api/person failed: {ex.Message}", ex, source: nameof(GetPeople), correlationId: CorrelationId);
                await _loggingService.LogRequestAsync("/api/person", "GET", 500, stopwatch.ElapsedMilliseconds, correlationId: CorrelationId);

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetPersonByName(string name)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await _loggingService.LogInformationAsync(Category, $"GET /api/person/{name} - Retrieving person by name", source: nameof(GetPersonByName), correlationId: CorrelationId);

                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _personAstronautService.GetPersonByName(name, CorrelationId, cancellationToken);

                stopwatch.Stop();
                await _loggingService.LogRequestAsync($"/api/person/{name}", "GET", result.ResponseCode, stopwatch.ElapsedMilliseconds, correlationId: CorrelationId);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                await _loggingService.LogErrorAsync(Category, $"GET /api/person/{name} failed: {ex.Message}", ex, source: nameof(GetPersonByName), correlationId: CorrelationId);
                await _loggingService.LogRequestAsync($"/api/person/{name}", "GET", 500, stopwatch.ElapsedMilliseconds, correlationId: CorrelationId);

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePerson([FromBody] PersonRequest request)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await _loggingService.LogInformationAsync(Category, $"POST /api/person - Creating person: {request.Name}", source: nameof(CreatePerson), correlationId: CorrelationId);

                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _personAstronautService.CreatePerson(request, CorrelationId, cancellationToken);

                stopwatch.Stop();
                await _loggingService.LogRequestAsync("/api/person", "POST", result.ResponseCode, stopwatch.ElapsedMilliseconds, correlationId: CorrelationId);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                await _loggingService.LogErrorAsync(Category, $"POST /api/person failed: {ex.Message}", ex, source: nameof(CreatePerson), correlationId: CorrelationId);
                await _loggingService.LogRequestAsync("/api/person", "POST", 500, stopwatch.ElapsedMilliseconds, correlationId: CorrelationId);

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpPut("{name}")]
        public async Task<IActionResult> UpdatePerson(string name, [FromBody] PersonRequest request)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await _loggingService.LogInformationAsync(Category, $"PUT /api/person/{name} - Updating person to: {request.Name}", source: nameof(UpdatePerson), correlationId: CorrelationId);

                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _personAstronautService.UpdatePerson(name, request, CorrelationId, cancellationToken);

                stopwatch.Stop();
                await _loggingService.LogRequestAsync($"/api/person/{name}", "PUT", result.ResponseCode, stopwatch.ElapsedMilliseconds, correlationId: CorrelationId);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                await _loggingService.LogErrorAsync(Category, $"PUT /api/person/{name} failed: {ex.Message}", ex, source: nameof(UpdatePerson), correlationId: CorrelationId);
                await _loggingService.LogRequestAsync($"/api/person/{name}", "PUT", 500, stopwatch.ElapsedMilliseconds, correlationId: CorrelationId);

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
