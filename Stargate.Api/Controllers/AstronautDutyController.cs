using System.Net;

using Microsoft.AspNetCore.Mvc;

using Stargate.Application.Interfaces;
using Stargate.Domain.Dtos;

namespace Stargate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AstronautDutyController : ControllerBase
    {
        private readonly IAstronautDutyService _astronautDutyService;

        public AstronautDutyController(IAstronautDutyService astronautDutyService)
        {
            _astronautDutyService = astronautDutyService;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetAstronautDutiesByName(string name)
        {
            try
            {
                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _astronautDutyService.GetAstronautDutiesByName(name, cancellationToken);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
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
                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _astronautDutyService.CreateAstronautDuty(request, cancellationToken);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
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
