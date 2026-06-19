namespace FitnessClub_Test.Dtos
{
    public class UserProfileDto
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public string? QrCode { get; set; }

        public string PhoneNumber { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public string MedicalIssues { get; set; }
        public string Target { get; set; }


        public decimal? Salary { get; set; }
        public int? Experience { get; set; }
        public string Specialty { get; set; }
    }
}
