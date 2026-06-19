using System;
using System.ComponentModel.DataAnnotations;

public class DateOfBirthAttribute : ValidationAttribute
{
    public int MaximumAge { get; set; } = 120;

    public DateOfBirthAttribute()
    {
        ErrorMessage = "Date of birth is invalid.";
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success; // [Required] handles null check

        if (value is DateOnly dob)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            if (dob > today)
                return new ValidationResult("Date of birth cannot be in the future.");

            int age = today.Year - dob.Year;
            if (dob > today.AddYears(-age)) age--;

            if (age > MaximumAge)
                return new ValidationResult($"Age cannot be more than {MaximumAge} years.");

            return ValidationResult.Success;
        }

        return new ValidationResult("Invalid date format.");
    }
}
