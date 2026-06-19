using System.Collections.Generic;

namespace FitnessClub_Test.Dtos
{
    public class DashboardDTO
    {
        public List<LatestMembersDTO> latestMembers;
        public int latestMembersCount { get; set; }
        public int TotalMembersCount { get; set; }
        public int TotalNumberUpcomingExpirations { get; set; }
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
    }
}
