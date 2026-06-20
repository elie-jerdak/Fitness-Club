using FitnessClub_Test.Dtos.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Dtos
{
    public class CoachCreateDTO
    {
        public int UserID { get; set; }
        
        [Required(ErrorMessage = "First Name is required.")]
        [DisplayName("First Name")]
        public string First_Name { get; set; }
        
        [Required(ErrorMessage = "Last Name is required.")]
        [DisplayName("Last Name")]
        public string Last_Name { get; set; }
        
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com)$", ErrorMessage = "Email must end with .com")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters and have a mix of uppercase, lowercase letters, numbers and symbols")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Date of birth is required")]
        [MiniumAge(18)]
        public DateOnly DOB { get; set; }
        
        [Required]
        public string Gender { get; set; }
        public string Role { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [DisplayName("Phone Number")]
        [RegularExpression(@"^\+?\d{1,3}?[-.\s]?\(?\d+\)?([-.\s]?\d+)*$", ErrorMessage = "Invalid phone number format.")]
        [MinLength(8, ErrorMessage = "Phone number is too short")]
        [MaxLength(15)]
        public string Phone_Number { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
        public string Photo { get; set; }
        [Required]
        public string QrCode { get; set; }

        public string ProfileImageBase64 { get; set; }
        public string ProfileImageFileName { get; set; }

        [Required]
        public int YearsExperience { get; set; }
        [Required]
        public decimal Salary { get; set; }
        [Required]
        public string Specialty { get; set; }
        public string Bio { get; set; }
    }
}
