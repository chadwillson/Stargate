namespace Stargate.Domain.Dtos
{
    public class PersonAstronautResponse : BaseResponse
    {
        public int PersonId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? CurrentRank { get; set; }

        public string? CurrentDutyTitle { get; set; }

        public DateTime? CareerStartDate { get; set; }

        public DateTime? CareerEndDate { get; set; }
    }
}
