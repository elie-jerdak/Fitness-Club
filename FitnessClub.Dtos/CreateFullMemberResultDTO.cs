using System.Collections.Generic;

namespace FitnessClub_Test.Dtos
{
    public class CreateFullMemberResultDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public int UserId { get; set; }
        public int ClientId { get; set; }
        public int MembershipId { get; set; }
        public int CoachID { get; set; }

        public List<string> Errors { get; set; } = new List<string>();
    }
}
