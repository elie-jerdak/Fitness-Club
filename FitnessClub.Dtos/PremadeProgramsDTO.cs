using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub_Test.Dtos
{
    public class PremadeProgramsDTO
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000, ErrorMessage = "Price must be greater than 0 and less than 1000")]
        public decimal? Price { get; set; }

        public string CoverImage { get; set; }

        public string CoverImageName { get; set; }
        public string CoverImageBase64 { get; set; }
        public string Owner { get; set; }

        [Required(ErrorMessage = "Equipments is required")]
        [DisplayName("Equipments Needed")]
        public string EquipmentNeeded { get; set; }

        [Range(1, 1000, ErrorMessage = "Duration must be at least 1")]
        public int? Duration { get; set; }

        [Required]
        public string Level { get; set; }
        
        [Required]
        public string Intensity { get; set; }

        [Required]
        [DisplayName("Number Of Exercises")]
        public int NumberOfExercises { get; set; }

        [Required]
        public string Benefits { get; set; }

        [Required(ErrorMessage = "Coach is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Coach ID")]
        public int? CoachID { get; set; }

        public List<ProgramExerciseDTO> Exercises { get; set; } = new();
    }

}
