using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Interfaces
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task<string> UploadFileAsync(byte[] fileBytes, string fileName);
    }
}