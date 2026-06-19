namespace FitnessClub_Test.Dtos
{
    public class UserProfileUpdateDto
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string PhoneNumber { get; set; }

        // Role specific fields:
        public string Role { get; set; }  // "Client" or "Coach"

        // Client fields
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public string MedicalIssues { get; set; }
        public string Target { get; set; }

        // Coach fields
        public decimal? Salary { get; set; }
        public int? Experience { get; set; }
        public string Specialty { get; set; }
    }

}
