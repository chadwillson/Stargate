using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stargate.Application.Interfaces;
using Stargate.Domain.Dtos;

namespace Stargate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class TestDataController : ControllerBase
    {
        private readonly ITestDataService _testDataService;
        private readonly ILogger<TestDataController> _logger;

        public TestDataController(ITestDataService testDataService, ILogger<TestDataController> logger)
        {
            _testDataService = testDataService;
            _logger = logger;
        }

        [HttpPost("scenarios/basic")]
        public async Task<IActionResult> CreateBasicScenario()
        {
            try
            {
                LogCreatingScenario("Basic");

                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _testDataService.CreateBasicScenarioAsync(cancellationToken);

                LogScenarioCreated("Basic", result.CreatedPersonIds.Count);
                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                LogScenarioCreationFailed("Basic", ex);
                return this.GetResponse(new BaseResponse
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpPost("scenarios/complex")]
        public async Task<IActionResult> CreateComplexScenario()
        {
            try
            {
                LogCreatingScenario("Complex");

                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _testDataService.CreateComplexScenarioAsync(cancellationToken);

                LogScenarioCreated("Complex", result.CreatedPersonIds.Count);
                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                LogScenarioCreationFailed("Complex", ex);
                return this.GetResponse(new BaseResponse
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpPost("scenarios/edgecase")]
        public async Task<IActionResult> CreateEdgeCaseScenario()
        {
            try
            {
                LogCreatingScenario("EdgeCase");

                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _testDataService.CreateEdgeCaseScenarioAsync(cancellationToken);

                LogScenarioCreated("EdgeCase", result.CreatedPersonIds.Count);
                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                LogScenarioCreationFailed("EdgeCase", ex);
                return this.GetResponse(new BaseResponse
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpPost("scenarios/retired")]
        public async Task<IActionResult> CreateRetiredAstronautScenario()
        {
            try
            {
                LogCreatingScenario("Retired");

                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _testDataService.CreateRetiredAstronautScenarioAsync(cancellationToken);

                LogScenarioCreated("Retired", result.CreatedPersonIds.Count);
                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                LogScenarioCreationFailed("Retired", ex);
                return this.GetResponse(new BaseResponse
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpPost("scenarios/multipleduties")]
        public async Task<IActionResult> CreateMultipleDutiesScenario()
        {
            try
            {
                LogCreatingScenario("MultipleDuties");

                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _testDataService.CreateMultipleDutiesScenarioAsync(cancellationToken);

                LogScenarioCreated("MultipleDuties", result.CreatedPersonIds.Count);
                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                LogScenarioCreationFailed("MultipleDuties", ex);
                return this.GetResponse(new BaseResponse
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> ClearAllTestData()
        {
            try
            {
                LogClearingAllData();

                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _testDataService.ClearAllTestDataAsync(cancellationToken);

                LogDataCleared();
                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                LogClearDataFailed(ex);
                return this.GetResponse(new BaseResponse
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetToDefaultSeedData()
        {
            try
            {
                LogResettingToDefaultData();

                CancellationToken cancellationToken = HttpContext.RequestAborted;
                var result = await _testDataService.ResetToDefaultSeedDataAsync(cancellationToken);

                LogDataReset();
                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                LogResetDataFailed(ex);
                return this.GetResponse(new BaseResponse
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }
    }
}
