using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub.Website.MVC.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly Dob { get; set; }

        [Required]
        public string Gender { get; set; }

        public string PhoneNumber { get; set; }

        [Required]
        public string Role { get; set; }

         // Client fields
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public string Target { get; set; }
        public string MedicalHistory { get; set; }

        // Coach fields
        public int? Experience { get; set; }
        public string Specialty { get; set; }
        public decimal? Salary { get; set; }
    
    }
}
