using FitnessClub_Test.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Interfaces
{
    public interface IExerciseService
    {
        Task<List<ExerciseDTO>> GetAllExercisesAsync();
        Task<ExerciseDTO> GetExerciseByIdAsync(int ExerciseID);
        Task<int> CreateExerciseAsync(ExerciseDTO dto);
        Task<bool> UpdateExerciseAsync(ExerciseDTO dto);
        Task<bool> DeleteExerciseAsync(int ExerciseID);
        Task BulkDeleteAsync(List<int> ids);
    }
}
