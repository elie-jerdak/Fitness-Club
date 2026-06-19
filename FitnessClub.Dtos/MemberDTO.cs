using System;

namespace FitnessClub_Test.Dtos
{
    public class MemberDTO
    {
        public int UserID { get; set; }
        public int MemberID { get; set; }
        public int ClientID { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Type { get; set; }
        public string Photo { get; set; }

    }
}
