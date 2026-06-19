using System;

namespace FitnessClub_Test.Dtos
{
    public class MessageDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public int UserId { get; set; }
        public DateTime? Time { get; set; }
    }
}
