using FitnessClub_Test.Dtos.Validations;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace FitnessClub_Test.Dtos
{
    public class UserDTO
    {
        public int Id { get; set; }


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
        public DateTime? DOB { get; set; }

        [Required(ErrorMessage = "Enter a valid gender")]
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

        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

        public string Photo { get; set; }

        public int? CalendarID;

        public string QrCode { get; set; }
    }
}
