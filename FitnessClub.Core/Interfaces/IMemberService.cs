using FitnessClub_Test.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Interfaces
{
    public interface IMemberService
    {
        Task<List<MemberDTO>> GetAllMembersAsync();
        Task<MemberEditDTO> GetMemberForUpdateAsync(int memberId);
        Task<(bool Success, Dictionary<string, string[]> Errors)> UpdateMemberAsync(int memberId, MemberEditDTO dto);
        Task<MemberDTO> GetMemberForDeleteAsync(int memberId);
        Task<bool> DeleteMemberAsync(int UserID);
        Task<BulkDeleteDTO> BulkDeleteMemberAsync(List<int> selectedMemberIDs);
        Task<CreateFullMemberResultDTO> CreateMemberAsync(FullMemberDto dto);
        Task<MemberEditDTO?> GetMemberByUserIdAsync(int userId);
        Task<List<PremadeProgramsDTO>> GetPurchasedProgramsAsync(int userId);
        Task<List<AvailabilitiesBookedDTO>> GetAvailabilitiesBooked(int userId);

    }
}
