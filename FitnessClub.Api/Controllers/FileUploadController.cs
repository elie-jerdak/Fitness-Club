using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FitnessClub_Test.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("file-upload")]
    public class FileUploadController : ControllerBase
    {
        private readonly FitnessClubDbContext _context;
        private readonly IFileUploadService _fileUploadService;

        public FileUploadController(IFileUploadService fileUploadService, FitnessClubDbContext context)
        {
            _fileUploadService = fileUploadService;
            _context = context;
        }

        [HttpPost("UploadFile")]
        public async Task<IActionResult> Upload([FromForm] UploadFileDTO dto)
        {
            Console.WriteLine($"UserId = {dto.UserId}");
            try
            {
                var relativePath = await _fileUploadService.UploadFileAsync(dto.file);
                Console.WriteLine($"relative path =  { relativePath} ");
                // Save path in DB
                var user = await _context.Users.FindAsync(dto.UserId);
                if (user == null)
                {
                    Console.WriteLine("Member from the upload file api Not Found");
                    return NotFound("Member not found");
                }

                Console.WriteLine("Before Saving");
                user.Photo = relativePath;
                
                await _context.SaveChangesAsync();
                Console.WriteLine("Save Completed");
                
                return Ok(new { filePath = relativePath });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message );
                return BadRequest(ex.Message);
            }
        }
    }
}