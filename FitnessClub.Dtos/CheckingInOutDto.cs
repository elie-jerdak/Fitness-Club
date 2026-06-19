using System;

namespace FitnessClub_Test.Core.DTOs
{
    public class CheckingInOutDto
    {
        public int Id { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public int UserId { get; set; }
    }
}
