using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub_Test.Dtos.Validations
{
    public class MiniumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;
        private readonly int _maximumAge = 120;
        public MiniumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
            ErrorMessage = $"You must be at least {_minimumAge} years old";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is not DateOnly dob)
                return new ValidationResult("Invalid date format");

            // Use UTC now for comparison
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (dob > today)
                return new ValidationResult("Date of birth cannot be in the future");

            var age = today.Year - dob.Year;

            // If birthday hasn't occurred yet this year, subtract 1
            if (dob > today.AddYears(-age))
                age--;

            if (age < _minimumAge)
                return new ValidationResult($"You must be at least {_minimumAge} years old");

            if (age > _maximumAge)
                return new ValidationResult($"Age cannot exceed {_maximumAge} years old");

            return ValidationResult.Success;
        }

    }
}
