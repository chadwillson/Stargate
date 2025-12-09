using System.Net;

using Microsoft.AspNetCore.Mvc;

using Stargate.Application.Interfaces;
using Stargate.Domain.Dtos;

namespace Stargate.Api.Controllers
{
    public class PersonController : Controller
    {
        private readonly IPersonAstronautService _personAstronautService;

        public PersonController(IPersonAstronautService personAstronautServic)
        {
            _personAstronautService = personAstronautServic;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetPeople()
        {
            try
            {
                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _personAstronautService.GetPeople(cancellationToken);

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

        [HttpGet("{name}")]
        public async Task<IActionResult> GetPersonByName(string name)
        {
            try
            {
                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _personAstronautService.GetPersonByName(name, cancellationToken);

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

        [HttpPost("")]
        public async Task<IActionResult> CreatePerson([FromBody] PersonRequest request)
        {
            try
            {
                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _personAstronautService.CreatePerson(request, cancellationToken);

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
