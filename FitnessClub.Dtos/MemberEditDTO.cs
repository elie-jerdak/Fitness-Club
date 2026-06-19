using FitnessClub_Test.Dtos.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub_Test.Dtos
{
    public class MemberEditDTO
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "First Name is required.")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Enter a valid gender")]
        public string Gender { get; set; }

        public string Role { get; set; }
        
        [Required]
        public string Address { get; set; }
        
        [Required(ErrorMessage = "Phone Number is required")]
        [DisplayName("Phone Number")]
        [RegularExpression(@"^\+?\d{1,3}?[-.\s]?\(?\d+\)?([-.\s]?\d+)*$", ErrorMessage = "Invalid phone number format.")]
        [MinLength(8, ErrorMessage = "Phone number is too short")]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        public string Photo {  get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [MiniumAge(18)]
        public DateTime? DOB { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com)$", ErrorMessage = "Email must end with .com")]
        public string Email { get; set; }
        public string QrCode { get; set; }

        // Client Info
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public string Target { get; set; }
        public string MedicalHistory { get; set; }

        // Membership Info
        public int MemberID { get; set; }
        public string Type { get; set; }
        public bool IsAutoRenewed { get; set; }

        // Profile Image
        public string ProfileImageBase64 { get; set; }
        public string ProfileImageFileName { get; set; }

        public List<PremadeProgramsDTO> PremadePrograms { get; set; } = new List<PremadeProgramsDTO>();
    }
}
