namespace FitnessClub_Test.Dtos
{
    public class FullMemberDto
    {
        
        public UserDTO user { get; set; }
        public ClientDTO client { get; set; }
        public MembershipDto membership { get; set; }
        public string ProfileImageBase64 { get; set; }
        public string ProfileImageFileName { get; set; }
    }
}
