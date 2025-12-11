namespace Stargate.Domain.Dtos
{
    public class TestDataResponse : BaseResponse
    {
        public string Scenario { get; set; } = string.Empty;
        public List<int> CreatedPersonIds { get; set; } = new();
        public List<int> CreatedDutyIds { get; set; } = new();
        public List<int> CreatedDetailIds { get; set; } = new();
        public string Description { get; set; } = string.Empty;
    }
}
