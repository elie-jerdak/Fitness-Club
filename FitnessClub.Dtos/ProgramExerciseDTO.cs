using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Dtos
{
    public class ProgramExerciseDTO
    {
        public int ExerciseID { get; set; }
        public string ExerciseName { get; set; }
        public int? NumberOfSets { get; set; }
        public int? NumberOfReps { get; set; }
        public int? Duration { get; set; }
    }
}
