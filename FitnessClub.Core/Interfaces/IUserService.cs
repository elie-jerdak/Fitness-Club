using System.Threading.Tasks;
using FitnessClub_Test.Dtos;

public interface IUserService
{
    public Task<UserProfileDto> UpdateUserByEmailAsync(string email, UserProfileUpdateDto updateDto);

    
}
