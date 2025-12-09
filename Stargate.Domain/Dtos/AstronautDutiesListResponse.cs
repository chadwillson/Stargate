namespace Stargate.Domain.Dtos
{
    public class AstronautDutiesListResponse : BaseResponse
    {
        public List<AstronautDutiesByNameResponse> Duties { get; set; } = new List<AstronautDutiesByNameResponse>();
    }
}
