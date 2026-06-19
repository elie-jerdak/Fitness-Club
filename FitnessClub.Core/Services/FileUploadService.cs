using FitnessClub.Core.Options;
using FitnessClub_Test.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IConfiguration _configuration;
        private readonly string _profileImagesPath;

        public FileUploadService(IConfiguration configuration, IOptions<StoragePathsOptions> options)
        {
            _configuration = configuration;
            _profileImagesPath = options.Value.ProfileImagesPath;
        }

        // this is not used so it may be buggy
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".docx", ".xlsx", ".zip" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                throw new Exception("File type not allowed.");

            //10 MB
            const long maxFileSize = 10 * 1024 * 1024;
            if (file.Length > maxFileSize)
                throw new Exception("File size exceeds limit.");

            // Path to MVC wwwroot/Images, 
            var targetPath = _configuration["Paths:SystemStorage"];

            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            var uniqueFileName = Guid.NewGuid() + extension;
            var fullPath = Path.Combine(targetPath, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return  fullPath; 
        }

        // this is what is being used
        public async Task<string> UploadFileAsync(byte[] fileBytes, string filename)
        {
            // 1. Resolve wwwroot path
            if (string.IsNullOrWhiteSpace(_profileImagesPath))
                throw new InvalidOperationException("ProfileImagesPath is not configured");             
                       
            // 2. Ensure directory exists
            if (!Directory.Exists(_profileImagesPath))
                Directory.CreateDirectory(_profileImagesPath);

            // 3. Sanitize + generate filename
            var extension = Path.GetExtension(filename);
            if (string.IsNullOrEmpty(extension))
                extension = ".png";

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            // 4. Absolute disk path
            var absolutePath = Path.Combine(_profileImagesPath, uniqueFileName);
            
            // 5. Save file
            await File.WriteAllBytesAsync(absolutePath, fileBytes);

            // 6. Return Unique File Name
            return uniqueFileName;
        }

    }

}
