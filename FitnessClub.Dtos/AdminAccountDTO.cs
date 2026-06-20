using FitnessClub_Test.Dtos.Validations;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub_Test.Dtos
{
    public class AdminAccountDTO
    {
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

        [Required(ErrorMessage = "Date of birth is required")]
        [MiniumAge(18)]
        public DateOnly DOB { get; set; }

        [Required(ErrorMessage = "Enter a valid gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [DisplayName("Phone Number")]
        [RegularExpression(@"^\+?\d{1,3}?[-.\s]?\(?\d+\)?([-.\s]?\d+)*$", ErrorMessage = "Invalid phone number format.")]
        [MinLength(8, ErrorMessage = "Phone number is too short")]
        [MaxLength(15)]
        public string Phone_Number { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Photo { get; set; }
        public string ProfileImageBase64 { get; set; }
        public string ProfileImageFileName { get; set; }
        public string QrCode { get; set; }
    }
}
