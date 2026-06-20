using FitnessClub_Test.Dtos.Validations;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub_Test.Dtos
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "First Name is required.")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com)$", ErrorMessage = "Email must end with .com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters and have a mix of uppercase, lowercase letters, numbers and symbols")]
        public string Password { get; set; }

        [Required, Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [MiniumAge(18)]
        public DateOnly Dob { get; set; }

        [Required(ErrorMessage = "Check One Of The Gender Options")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [DisplayName("Phone Number")]
        [RegularExpression(@"^\+?\d{1,3}?[-.\s]?\(?\d+\)?([-.\s]?\d+)*$", ErrorMessage = "Invalid phone number format.")]
        [MinLength(8, ErrorMessage = "Phone number is too short.")]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
    }
}
