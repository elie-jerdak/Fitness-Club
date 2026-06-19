using FitnessClub_Test.Dtos;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Interfaces
{
    public interface IAdminAccountService
    {
        Task<AdminAccountDTO?> GetAdminByIdAsync(int userId);
        Task<(bool Success, string Error)> UpdateAsync(int userId, AdminAccountDTO dto);
    }
}
