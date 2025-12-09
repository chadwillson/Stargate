namespace Stargate.Domain.Dtos
{
    public class PersonAstronautListResponse : BaseResponse
    {
        public List<PersonAstronautResponse> People { get; set; } = new List<PersonAstronautResponse>();
    }
}
