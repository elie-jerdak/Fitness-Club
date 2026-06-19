using System;
using System.Threading.Tasks;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly FitnessClubDbContext _dbContext;

    public UserService(FitnessClubDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<UserProfileDto> UpdateUserByEmailAsync(string email, UserProfileUpdateDto updateDto)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            throw new Exception("User not found");

        // Update common fields
      

        //if (user.Role == "Client")
        //{
        //    var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.UserId == user.Id);
        //    if (client == null)
        //    {
        //        client = new Client { UserId = user.Id };
        //        _dbContext.Clients.Add(client);
        //    }

        //    client.Weight = updateDto.Weight;
        //    client.Height = updateDto.Height;
        //    client.MedicalHistory = updateDto.MedicalIssues;
        //    client.Target = updateDto.Target;
        //}
        //else if (user.Role == "Coach")
        //{
        //    var coach = await _dbContext.Coaches.FirstOrDefaultAsync(c => c.UserId == user.Id);
        //    if (coach == null)
        //    {
        //        coach = new Coach { UserId = user.Id };
        //        _dbContext.Coaches.Add(coach);
        //    }

        //    coach.Salary = updateDto.Salary;
        //    coach.Experience = updateDto.Experience;
        //    coach.Specialty = updateDto.Specialty;
        //}

        await _dbContext.SaveChangesAsync();

        // Build profile DTO
        var profileDto = new UserProfileDto
        {
            FirstName = user.FirstName,
            SecondName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            //Role = user.Role,
            QrCode = user.QrCode
        };

        //if (user.Role == "Client")
        //{
        //    var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.UserId == user.Id);
        //    profileDto.Weight = client?.Weight;
        //    profileDto.Height = client?.Height;
        //    profileDto.MedicalIssues = client?.MedicalHistory;
        //    profileDto.Target = client?.Target;
        //}
        //else if (user.Role == "Coach")
        //{
        //    var coach = await _dbContext.Coaches.FirstOrDefaultAsync(c => c.UserId == user.Id);
        //    profileDto.Salary = coach?.Salary;
        //    profileDto.Experience = coach?.Experience;
        //    profileDto.Specialty = coach?.Specialty;
        //}

        return profileDto;
    }


}