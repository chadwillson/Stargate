using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stargate.Application.Interfaces;
using Stargate.Domain.Dtos;

namespace Stargate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class PersonController : ControllerBase
    {
        private readonly IPersonAstronautService _personAstronautService;
        private readonly ILogger<PersonController> _logger;

        public PersonController(IPersonAstronautService personAstronautService, ILogger<PersonController> logger)
        {
            _personAstronautService = personAstronautService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetPeople()
        {
            try
            {
                LogRetrievingAllPeople();

                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _personAstronautService.GetPeople(cancellationToken);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                LogFailedToRetrieveAllPeople(ex);
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
            try
            {
                LogRetrievingPersonByName(name);

                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _personAstronautService.GetPersonByName(name, cancellationToken);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                LogFailedToRetrievePersonByName(ex, name);
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
            try
            {
                LogCreatingPerson(request.Name);

                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _personAstronautService.CreatePerson(request, cancellationToken);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                LogFailedToCreatePerson(ex, request.Name);
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
            try
            {
                LogUpdatingPerson(name, request.Name);

                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _personAstronautService.UpdatePerson(name, request, cancellationToken);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                LogFailedToUpdatePerson(ex, name);
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
