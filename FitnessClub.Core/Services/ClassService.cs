using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Services
{
    public class ClassService : IClassService
    {
        private readonly FitnessClubDbContext _context;
        private readonly ILogger<ClassService> _logger;

        public ClassService(FitnessClubDbContext context, ILogger<ClassService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ClassDTO>> GetAllClasses()
        {
            try
            {
                var classes = await _context.Classes
                    .Where(c => !c.IsDeleted && c.IsActive)
                    .Include(c => c.Coach)
                        .ThenInclude(coach => coach.User)
                    .ToListAsync();

                return classes.Select(x => new ClassDTO
                {
                    ClassID = x.Id,
                    Name = x.Name,
                    Type = x.Type,
                    StartDate = x.StartDate,
                    EndDate = x.EndTime,
                    Description = x.Description,
                    Status = x.Status,
                    MaxOccupancy = x.MaxOccupancy,
                    Recurrence = x.Reccurence,
                    CoachID = x.CoachId,
                    CoachName = x.Coach != null && x.Coach.User != null
                        ? $"{x.Coach.User.FirstName} {x.Coach.User.LastName}"
                        : "No Coach Assigned"
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all classes.");
                return new List<ClassDTO>();
            }
        }

        public async Task<ClassDTO> GetClassById(int ClassID)
        {
            try
            {
                var cls = await _context.Classes
                    .Where(c => !c.IsDeleted && c.IsActive)
                    .Include(c => c.Coach)
                        .ThenInclude(coach => coach.User)
                    .FirstOrDefaultAsync(c => c.Id == ClassID); 

                if (cls == null)
                    return null;

                return new ClassDTO
                {
                    ClassID = cls.Id,
                    Name = cls.Name,
                    Type = cls.Type,
                    Fee = cls.Fee,
                    MaxOccupancy = cls.MaxOccupancy,
                    Description = cls.Description,
                    Status = cls.Status,
                    StartDate = cls.StartDate,
                    EndDate = cls.EndTime,
                    Recurrence = cls.Reccurence,
                    CoachID = cls.CoachId,
                    CoachName = cls.Coach != null && cls.Coach.User != null
                        ? $"{cls.Coach.User.FirstName} {cls.Coach.User.LastName}"
                        : "No Coach Assigned"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class by ID.");
                return null;
            }
        }

        public async Task<ClassDTO> CreateClass(ClassDTO dto)
        {
            try
            {
                // 🔒 VALIDATION
                if (dto.StartDate >= dto.EndDate)
                    throw new Exception("End time must be after start time.");

                // Check coach exists
                var coach = await _context.Coaches
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == dto.CoachID);

                if (coach == null)
                    throw new Exception("Invalid coach ID.");

                // CREATE ENTITY
                var newClass = new Class
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Type = dto.Type,
                    Reccurence = dto.Recurrence,
                    Status = "Scheduled",
                    StartDate = dto.StartDate,
                    EndTime = dto.EndDate,
                    Fee = dto.Fee,
                    MaxOccupancy = dto.MaxOccupancy,
                    CoachId = dto.CoachID,
                    IsActive = true,
                    IsDeleted = false
                };

                // SAVE
                _context.Classes.Add(newClass);
                await _context.SaveChangesAsync();

                // MAP BACK TO DTO
                return new ClassDTO
                {
                    ClassID = newClass.Id,
                    Name = newClass.Name,
                    Description = newClass.Description,
                    Type = newClass.Type,
                    Recurrence = newClass.Reccurence,
                    StartDate = newClass.StartDate,
                    EndDate = newClass.EndTime,
                    Fee = newClass.Fee,
                    MaxOccupancy = newClass.MaxOccupancy ?? 0,
                    CoachID = newClass.CoachId,

                    // Return coach name (better for UI)
                    CoachName = $"{coach.User.FirstName} {coach.User.LastName}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new class.");
                return null;
            }
        }

        public async Task<bool> UpdateClass(ClassDTO dto)
        {
            try
            {
                var existing = await _context.Classes.FindAsync(dto.ClassID);

                if (existing == null || existing.IsDeleted)
                    return false;

                // Core fields
                existing.Name = dto.Name;
                existing.Type = dto.Type;
                existing.Description = dto.Description;
                existing.Reccurence = dto.Recurrence;

                // Dates
                existing.StartDate = dto.StartDate;
                existing.EndTime = dto.EndDate;

                // Numeric fields
                existing.Fee = dto.Fee;
                existing.MaxOccupancy = dto.MaxOccupancy;
                existing.Status = dto.Status;

                // Relationship
                existing.CoachId = dto.CoachID;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating class with ID {ClassID}", dto.ClassID);
                return false;
            }
        }

        public async Task<bool> DeleteClass(int id)
        {
            try
            {
                var existing = await _context.Classes.FindAsync(id);
                if (existing == null || existing.IsDeleted)
                    return false;

                existing.IsDeleted = true;
                existing.IsActive = false;

                _context.Classes.Update(existing);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting class.");
                return false;
            }
        }
    }
}
