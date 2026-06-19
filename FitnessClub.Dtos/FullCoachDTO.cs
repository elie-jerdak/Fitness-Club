using System.Collections.Generic;

namespace FitnessClub_Test.Dtos
{
    public class FullCoachDTO
    {
        public UserDTO user {  get; set; }
        public CoachDTO coach { get; set; }
        public List<AvailabilityDTO> availability { get; set; }
        public double Rating { get; set; }  
        public int FeedbackCount { get; set; }

        // list of individual reviews
        public List<FeedbackDTO> Reviews { get; set; }

        public List<PremadeProgramsDTO> programs { get; set; }
    }
}
