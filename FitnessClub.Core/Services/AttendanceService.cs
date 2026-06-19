using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly FitnessClubDbContext _context;
        private readonly TimeSpan _staleThreshold = TimeSpan.FromHours(24); // adjust as needed

        public AttendanceService(FitnessClubDbContext context)
        {
            _context = context;
        }

        public async Task<string> ResolveNextQrType(int userId)
        {
            // Call API or DB to get last session
            var lastSession = await _context.CheckingInOuts
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.TimeIn)
            .FirstOrDefaultAsync();

            var now = DateTime.UtcNow;

            // Case 1: first visit → no attendance record
            if (lastSession == null)
                return "IN";

            // Case 2: last session checked out normally
            if (lastSession.TimeOut != null)
                return "IN";

            // Case 3: last session still active but stale
            if ((now - lastSession.TimeIn) > _staleThreshold)
                return "IN";

            // Case 4: last session still active → must check out
            return "OUT";
        }
    }

}
