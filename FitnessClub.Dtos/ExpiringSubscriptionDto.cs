using System;

namespace FitnessClub_Test.Dtos
{
    public class ExpiringSubscriptionDto
    {
        public int UserId { get; set; }
        public int MembershipId { get; set; }
        public string FullName { get; set; }
        public string Membership_Type { get; set; }
        public DateOnly EndDate { get; set; }
        public int DaysRemaining { get; set; }
        public bool NotificationSent { get; set; } 
    }
}
