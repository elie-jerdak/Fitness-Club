namespace FitnessClub_Test.Core.NewModels
{
    public class PremadeProgramExercise
    {
        public int PremadeProgramId { get; set; }
        public PremadeProgram PremadeProgram { get; set; }

        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; }

        public int NumberOfSets { get; set; }

        public int NumberOfReps { get; set; }
        public int Duration { get; set; }
    }

}
