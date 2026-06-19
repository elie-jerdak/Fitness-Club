using FitnessClub_Test.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Interfaces
{
    public interface IPremadeProgramsService
    {
        Task<List<PremadeProgramsDTO>> GetPremadeProgramsAsync();
        Task<bool> DeleteProgram(int id);
        Task<PremadeProgramsDTO?> GetPremadeProgramByIdAsync(int id);
        Task<List<PremadeProgramsDTO>> GetPremadeProgramsByCoachIdAsync(int coachId);
        Task<bool> UpdatePremadeProgramAsync(PremadeProgramsDTO dto);
        Task<bool> CreatePremadeProgramAsync(PremadeProgramsDTO dto);
    }
}
