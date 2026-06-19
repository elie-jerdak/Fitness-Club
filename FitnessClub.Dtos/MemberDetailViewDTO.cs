using System.Collections.Generic;

namespace FitnessClub_Test.Dtos
{
    public class MemberDetailViewDTO
    {
        public MemberEditDTO Member { get; set; }
        public List<PremadeProgramsDTO> PurchasedPrograms { get; set; }
        public List<AvailabilitiesBookedDTO> AvailabilitiesBooked { get; set; }
    }

}
