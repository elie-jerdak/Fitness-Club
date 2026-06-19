using System.ComponentModel.DataAnnotations;

namespace FitnessClub_Test.Dtos
{
    public class ClientDTO
    {
        [Required]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Only decimal numbers are allowed")]
        public decimal? Height { get; set; }
        [Required]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Only decimal numbers are allowed")]
        public decimal? Weight { get; set; }
        [Required]
        public string Target { get; set; }
        
        public string MedicalHistory { get; set; }

        public int UserId { get; set; }
    }
}
