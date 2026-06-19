using FitnessClub_Test.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Interfaces
{
   public interface IClassService
    {
        Task<List<ClassDTO>> GetAllClasses();
        Task<ClassDTO> GetClassById(int id);
        Task<ClassDTO> CreateClass(ClassDTO dto);
        Task<bool> UpdateClass(ClassDTO dto);
        Task<bool> DeleteClass(int id);
    }
}