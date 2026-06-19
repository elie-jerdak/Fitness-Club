using FitnessClub_Test.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Interfaces
{
    public interface ICoachService
    {
        Task<List<CoachDTO>> GetCoachesAsync();
        Task<CoachEditDTO> EditCoach(int userId);
        Task<bool> UpdateCoachAsync(CoachEditDTO dto);
        Task<CoachDTO> DeleteCoachAsync(int UserID);
        Task<bool> ConfirmDelete(int UserID);
        Task<BulkDeleteDTO> BulkDelete(List<int> selectedIds);
        Task<CreateFullMemberResultDTO> ConfirmCreate(CoachCreateDTO coachCreateDTO);
        Task<FullCoachDTO> GetCoachDetails(int userId);
        Task<bool> FreeTimeSlotAsync(int availabilityId, int clientId);
        Task<int> ReserveSlot(int availabilityId, int clientId);
        Task<int> CreateAvailability(CreateAvailabilityViewModel dto);
        Task<bool> DeleteAvailabilityAsync(int availabilityId);
        Task<AvailabilityDTO?> GetAvailabilityById(int id);
        Task<bool> UpdateAvailability(AvailabilityDTO dto);
        Task<int> GetCoachUserIdByAvailabilityId(int availabilityId);
    }
}