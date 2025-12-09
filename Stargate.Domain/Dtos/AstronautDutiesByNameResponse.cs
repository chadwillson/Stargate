namespace Stargate.Domain.Dtos
{
    public class AstronautDutiesByNameResponse : BaseResponse
    {
        public PersonAstronautResponse? Person { get; set; }
        public List<AstronautDutyResponse> AstronautDuties { get; set; } = new List<AstronautDutyResponse>();
    }
}
