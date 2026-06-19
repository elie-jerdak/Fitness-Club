using System.ComponentModel.DataAnnotations;

namespace FitnessClub_Test.Dtos
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com)$", ErrorMessage = "Email must end with .com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(15,MinimumLength = 8, ErrorMessage = "Password must be between 8 and 15 characters.")]
        public string Password { get; set; }
    }
}
