using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub_Test.Dtos
{
    public class ClassDTO
    {
        public int ClassID { get; set; }

        [Required]
        public string Name { get; set; }

        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Recurrence { get; set; }
        public string Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Max occupancy must be at least 1")]
        public int? MaxOccupancy { get; set; }

        [Range(0, 999999)]
        public decimal Fee { get; set; }

        [Required]
        public int CoachID { get; set; }

        public string CoachName { get; set; }
    }
}
