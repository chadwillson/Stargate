using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stargate.Application.Interfaces;
using Stargate.Domain.Dtos;

namespace Stargate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class AstronautDutyController : ControllerBase
    {
        private readonly IAstronautDutyService _astronautDutyService;
        private readonly ILogger<AstronautDutyController> _logger;

        public AstronautDutyController(IAstronautDutyService astronautDutyService, ILogger<AstronautDutyController> logger)
        {
            _astronautDutyService = astronautDutyService;
            _logger = logger;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetAstronautDutiesByName(string name)
        {
            try
            {
                LogRetrievingAstronautDuties(name);

                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _astronautDutyService.GetAstronautDutiesByName(name, cancellationToken);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                LogFailedToRetrieveAstronautDuties(ex, name);
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
            try
            {
                LogCreatingAstronautDuty(request.Name, request.DutyTitle);

                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _astronautDutyService.CreateAstronautDuty(request, cancellationToken);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                LogFailedToCreateAstronautDuty(ex, request.Name);
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
