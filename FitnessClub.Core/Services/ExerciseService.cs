using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly FitnessClubDbContext _context;

        public ExerciseService(FitnessClubDbContext context)
        {
            _context = context;
        }
        public async Task<List<ExerciseDTO>> GetAllExercisesAsync()
        {
            return await _context.Exercises
                .Where(e => !e.IsDeleted && e.IsActive)
                .Select(e => new ExerciseDTO
                {
                    ExerciseID = e.Id,
                    Name = e.Name
                })
                .ToListAsync();
        }

        public async Task<ExerciseDTO> GetExerciseByIdAsync(int ExerciseID)
        {
            var exercise = await _context.Exercises.FindAsync(ExerciseID);
            if (exercise == null || exercise.IsDeleted || !exercise.IsActive) return null;

            return new ExerciseDTO
            {
                ExerciseID = exercise.Id,
                Name = exercise.Name
            };
        }

        public async Task<int> CreateExerciseAsync(ExerciseDTO dto)
        {
            var exercise = new Exercise
            {
                Name = dto.Name,
                IsActive = true,
                IsDeleted = false
            };

            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();

            return exercise.Id;
        }

        public async Task<bool> UpdateExerciseAsync(ExerciseDTO dto)
        {
            var exercise = await _context.Exercises
        .FirstOrDefaultAsync(e => e.Id == dto.ExerciseID);

            if (exercise == null)
                return false;

            exercise.Name = dto.Name;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteExerciseAsync(int ExerciseID)
        {
            var exercise = await _context.Exercises.FirstOrDefaultAsync(e => e.Id == ExerciseID);

            if (exercise == null)
                return false;

            exercise.IsDeleted = true;
            exercise.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task BulkDeleteAsync(List<int> ids)
        {
            var exercises = await _context.Exercises
                .Where(e => ids.Contains(e.Id) && !e.IsDeleted) // only active exercises
                .ToListAsync();

            if (!exercises.Any())
                return; // Nothing to delete

            // Soft delete: mark as deleted
            foreach (var exercise in exercises)
            {
                exercise.IsDeleted = true;
                exercise.IsActive= false;
            }

            await _context.SaveChangesAsync();
        }
    }
}
