using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Interfaces
{
    public interface IScanService
    {
        Task<(bool Success, string Message)> ScanAsync(string token, string ip, string device);
    }
}