namespace Stargate.Domain.Dtos
{
    public class CreateAstronautDutyResponse : AstronautDutyBaseResponse
    {
        public required string Name { get; set; }

        public required string Rank { get; set; }

        public required string DutyTitle { get; set; }

        public DateTime DutyStartDate { get; set; }
    }
}
