using System;

namespace FitnessClub_Test.Dtos
{
    public class AvailabilityDTO
    {
        public int Id { get; set; }
        public int CoachId { get; set; }
        public DateOnly Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Status { get; set; }
        public int? ClientID { get; set; }
        public string? ClientFullName { get; set; }
    }
}