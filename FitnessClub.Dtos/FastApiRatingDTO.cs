using System.Collections.Generic;

namespace FitnessClub_Test.Dtos
{
    public class FastApiRatingDTO
    {
        public int coach_id { get; set; }
        public int feedback_count { get; set; }
        public double average_stars { get; set; }
        public List<FeedbackDTO> reviews { get; set; }
    }
}