using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Services
{
    public class PremadeProgramsService: IPremadeProgramsService
    {
        private readonly FitnessClubDbContext _context;
        private readonly IFileUploadService _fileUploadService;

        public PremadeProgramsService(FitnessClubDbContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        public async Task<List<PremadeProgramsDTO>> GetPremadeProgramsAsync()
        {
            var result = await _context.PremadePrograms
                .Where(c => c.IsDeleted == false && c.IsActive == true)
                .Include(c => c.Coach) 
                .Select(c => new PremadeProgramsDTO
                {
                    ID = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Category = c.Category,
                    Duration = c.Duration,
                    Owner = c.Coach.User.FirstName + " " + c.Coach.User.LastName,
                    Price = c.Price
                })
                .OrderByDescending(p => p.Price ?? 0) // null-safe ordering
                .ToListAsync();

            return result;
        }

        public async Task<bool> DeleteProgram(int id)
        {
            var program = await _context.PremadePrograms.FindAsync(id);

            if (program == null)
                return false;

            program.IsDeleted = true;
            program.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<PremadeProgramsDTO?> GetPremadeProgramByIdAsync(int programId)
        {
            var program = await _context.PremadePrograms
                .Include(p => p.Coach)
                    .ThenInclude(c => c.User)
                .Include(p => p.PremadeProgramExercises) // include the exercises
                    .ThenInclude(pe => pe.Exercise)       // include the Exercise entity for name, etc.
                .Where(p => p.Id == programId && p.IsDeleted == false)
                .Select(p => new PremadeProgramsDTO
                {
                    ID = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Category = p.Category,
                    EquipmentNeeded = p.EquipmentNeeded,
                    CoverImage = p.CoverImage,
                    Duration = p.Duration,
                    CoachID = p.CoachId,
                    Price = p.Price,
                    Level = p.Level,
                    Intensity = p.Intensity,
                    Benefits = p.Benefits,
                    NumberOfExercises = p.NumberOfExercises,
                    Exercises = p.PremadeProgramExercises
                        .Select(pe => new ProgramExerciseDTO
                        {
                            ExerciseID = pe.ExerciseId,
                            ExerciseName = pe.Exercise.Name,
                            NumberOfSets = pe.NumberOfSets,
                            NumberOfReps = pe.NumberOfReps,
                            Duration = pe.Duration
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return program;
        }

        public async Task<List<PremadeProgramsDTO>> GetPremadeProgramsByCoachIdAsync(int coachId)
        {
            var programs = await _context.PremadePrograms
                .Include(p => p.Coach)
                    .ThenInclude(c => c.User)
                .Where(p => p.CoachId == coachId && !p.IsDeleted)
                .Select(p => new PremadeProgramsDTO
                {
                    ID = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Category = p.Category,
                    EquipmentNeeded = p.EquipmentNeeded,
                    CoverImage = p.CoverImage,
                    Duration = p.Duration,
                    Owner = p.Coach.User.FirstName + " " + p.Coach.User.LastName,
                    Price = p.Price
                })
                .ToListAsync();

            return programs;
        }

        public async Task<bool> UpdatePremadeProgramAsync(PremadeProgramsDTO dto)
        {
            var program = await _context.PremadePrograms
                .Include(p => p.Coach)
                    .ThenInclude(c => c.User)
                .Include(p => p.PremadeProgramExercises)
                .FirstOrDefaultAsync(p => p.Id == dto.ID);

            if (program == null)
            {
                return false;
            }

            // Update main fields
            program.Title = dto.Title?.Trim();
            program.Category = dto.Category?.Trim();
            program.Description = dto.Description?.Trim();
            program.Level = dto.Level;
            program.Intensity = dto.Intensity;
            program.CoachId = dto.CoachID;
            program.IsPaid = dto.Price > 0 ? true : false;
            program.Benefits = dto.Benefits;
            program.NumberOfExercises = dto.Exercises.Count();
            program.EquipmentNeeded = dto.EquipmentNeeded?.Trim();
            program.Price = dto.Price;
            program.Duration = dto.Duration;

            // ===== Safe exercises handling =====
            var exercises = dto.Exercises ?? new List<ProgramExerciseDTO>();

            program.NumberOfExercises = exercises.Count;

            // ===== Remove ALL old exercises (even if new list is empty) =====
            _context.PremadeProgramExercises.RemoveRange(program.PremadeProgramExercises);

            // ===== Add new ones =====
            var newExercises = exercises.Select(ex => new PremadeProgramExercise
            {
                PremadeProgramId = program.Id,   // important
                ExerciseId = ex.ExerciseID,
                NumberOfSets = ex.NumberOfSets ?? 0,
                NumberOfReps = ex.NumberOfReps ?? 0,
                Duration = ex.Duration ?? 0
            }).ToList();

            await _context.PremadeProgramExercises.AddRangeAsync(newExercises);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CreatePremadeProgramAsync(PremadeProgramsDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var program = new PremadeProgram
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Category = dto.Category,
                    Price = dto.Price,
                    EquipmentNeeded = dto.EquipmentNeeded,
                    CoachId = dto.CoachID,
                    IsPaid = dto.Price.HasValue && dto.Price.Value > 0,
                    CoverImage = dto.CoverImage,
                    Duration = dto.Duration,
                    Level = dto.Level,
                    Intensity = dto.Intensity,
                    Benefits = dto.Benefits,
                    IsActive = true,
                    IsDeleted = false
                };
                
                if (dto.Exercises != null && dto.Exercises.Any())
                {
                    program.NumberOfExercises = dto.Exercises.Count;
                }

                _context.PremadePrograms.Add(program);
                await _context.SaveChangesAsync();

                // Loop to add exercises
                if (dto.Exercises != null && dto.Exercises.Any())
                {
                    foreach (var ex in dto.Exercises)
                    {
                        var programExercise = new PremadeProgramExercise
                        {
                            PremadeProgramId = program.Id, // FK
                            ExerciseId = ex.ExerciseID,
                            NumberOfSets = ex.NumberOfSets ?? 0,
                            NumberOfReps = ex.NumberOfReps ?? 0,
                            Duration = ex.Duration ?? 0
                        };

                        _context.PremadeProgramExercises.Add(programExercise);
                    }
                    Console.WriteLine("Before SaveChanges CoverImage: " + program.CoverImage);
                    await _context.SaveChangesAsync();
                }
                Console.WriteLine("Before commitAsync CoverImage: " + program.CoverImage);
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Log the error somewhere if needed
                Console.WriteLine("Error creating program: " + ex.Message);
                return false; // Failure
            }

        }

    }
}
