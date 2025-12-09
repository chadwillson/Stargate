using Microsoft.AspNetCore.Mvc;

using Stargate.Domain.Dtos;

namespace Stargate.Api.Controllers
{
    public static class ControllerBaseExtensions
    {
        public static IActionResult GetResponse(this ControllerBase controllerBase, BaseResponse response)
        {
            var httpResponse = new ObjectResult(response);
            httpResponse.StatusCode = response.ResponseCode;
            return httpResponse;
        }
    }
}
