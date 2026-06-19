using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub_Test.Dtos
{
    public class MembershipDto
    {
        public int memberID { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        [Required]
        public string Type { get; set; }
        public bool IsAutoRenewed { get; set; }
    }
}
