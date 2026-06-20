using System;
using System.Linq;
using System.Threading.Tasks;
using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Core.DTOs;
using Microsoft.EntityFrameworkCore;

public class CheckInOutService : ICheckInOutService
{
    private readonly FitnessClubDbContext _dbContext;

    public CheckInOutService(FitnessClubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CheckingInOutDto> CheckInOrOutAsync(int userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null) throw new Exception("User not found");

        var latest = await _dbContext.CheckingInOuts
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.TimeIn)
            .FirstOrDefaultAsync();

        if (latest == null || latest.TimeOut != null)
        {
            // No open session → check in
            var checkIn = new CheckingInOut
            {
                UserId = userId,
                TimeIn = DateTime.UtcNow
            };

            _dbContext.CheckingInOuts.Add(checkIn);
            await _dbContext.SaveChangesAsync();

            return new CheckingInOutDto
            {
                Id = checkIn.Id,
                UserId = checkIn.UserId,
                TimeIn = checkIn.TimeIn,
                TimeOut = null
            };
        }
        else
        {
            // Open session exists → check out
            latest.TimeOut = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return new CheckingInOutDto
            {
                Id = latest.Id,
                UserId = latest.UserId,
                TimeIn = latest.TimeIn,
                TimeOut = latest.TimeOut
            };
        }
    }

}
