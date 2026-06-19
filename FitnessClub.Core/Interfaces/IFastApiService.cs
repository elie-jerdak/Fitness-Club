using FitnessClub_Test.Dtos;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Interfaces
{
    public interface IFastApiService
    {
        Task<FastApiRatingDTO> GetCoachRatingAsync(int coachId);
    }
}