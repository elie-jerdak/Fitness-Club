using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Interfaces
{
    public interface IAttendanceService
    {
        Task<string> ResolveNextQrType(int userId);
    }
}
