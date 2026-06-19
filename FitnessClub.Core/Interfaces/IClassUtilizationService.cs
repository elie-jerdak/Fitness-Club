using FitnessClub_Test.Dtos;
using System.Collections.Generic;

namespace FitnessClub_Test.Core.Interfaces
{
    public interface IClassUtilizationService
    {
        List<ClassUtilizationDTO> ClassUtilization();
    }
}