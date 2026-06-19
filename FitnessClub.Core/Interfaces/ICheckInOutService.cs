using FitnessClub_Test.Core.DTOs;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Interfaces
{
    public interface ICheckInOutService
    {
        Task<CheckingInOutDto> CheckInOrOutAsync(int userId);
    }
}
