using System.ComponentModel.DataAnnotations;

namespace FitnessClub_Test.Dtos
{
    public class CoachDTO
    {
        public int CoachID { get; set; }
        
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get;set; }

        [Required]
        [Display(Name = "Years of Experience")]
        [Range(0, 60, ErrorMessage = "Years of experience must be between 0 and 60.")]
        public int? YearsExperience { get; set; }
        
        [Required(ErrorMessage = "Specialty is required.")]
        [StringLength(100, ErrorMessage = "Specialty cannot exceed 100 characters.")]
        public string Specialty { get; set; }
       
        [StringLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters.")]
        public string Bio { get; set; }
       
        [Range(0, 100000, ErrorMessage = "Salary must be a positive value.")]
        [DataType(DataType.Currency)]
        public decimal? Salary { get; set; }
        public int UserID { get; set; }
    }
}
