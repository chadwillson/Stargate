using System.Net;

using Microsoft.AspNetCore.Mvc;

using Stargate.Application.Interfaces;
using Stargate.Domain.Dtos;

namespace Stargate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonAstronautService _personAstronautService;

        public PersonController(IPersonAstronautService personAstronautServic)
        {
            _personAstronautService = personAstronautServic;
        }

        [HttpGet]
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

        [HttpPost]
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

        [HttpPut("{name}")]
        public async Task<IActionResult> UpdatePerson(string name, [FromBody] PersonRequest request)
        {
            try
            {
                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _personAstronautService.UpdatePerson(name, request, cancellationToken);

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
