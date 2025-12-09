namespace Stargate.Domain.Dtos
{
    public class AstronautDetailResponse : BaseResponse
    {
        public int Id { get; set; }

        public int PersonId { get; set; }

        public string CurrentRank { get; set; } = string.Empty;

        public string CurrentDutyTitle { get; set; } = string.Empty;

        public DateTime CareerStartDate { get; set; }

        public DateTime? CareerEndDate { get; set; }

        public virtual PersonAstronautResponse? Person { get; set; }
    }
}
    