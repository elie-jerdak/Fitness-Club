using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FitnessClub_Test.Core.Services
{
    public class ClassUtilizationService : IClassUtilizationService
    {
        private readonly FitnessClubDbContext _context;
        public ClassUtilizationService(FitnessClubDbContext context)
        { 
            _context = context;
        }
        public List<ClassUtilizationDTO> ClassUtilization()
        {
            var now = DateTime.UtcNow;
            int currentMonth = now.Month;
            int currentYear = now.Year;

            var result = new List<ClassUtilizationDTO>();

            var allClasses = _context.Classes
                .Where(c => !c.IsDeleted && c.Status != "Cancelled")
                .Select(c => new { c.Id, c.Name, c.MaxOccupancy })
                .ToList();

            foreach (var classItem in allClasses)
            {
                var attendanceCount = _context.Bookings
                    .Where(b => !b.IsDeleted &&
                                b.ClassId == classItem.Id &&
                                b.Date.HasValue &&
                                b.Date.Value.Month == currentMonth &&
                                b.Date.Value.Year == currentYear)
                    .Count();

                var utilizationRate = classItem.MaxOccupancy == 0 ? 0 :
                    Math.Round((decimal)((decimal)attendanceCount / classItem.MaxOccupancy * 100), 2);

                result.Add(new ClassUtilizationDTO
                {
                    ClassName = classItem.Name,
                    Attendance = attendanceCount,
                    Capacity = (int)classItem.MaxOccupancy,
                    UtilizationRate = utilizationRate
                });
            }

            return result;
        }

    }

}
