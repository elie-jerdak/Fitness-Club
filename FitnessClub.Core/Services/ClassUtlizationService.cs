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

            var start = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddMonths(1);

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
                                b.Date.Value >= start &&
                                b.Date.Value < end)
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
