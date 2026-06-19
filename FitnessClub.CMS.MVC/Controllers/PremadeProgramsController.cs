using FitnessClub_Test.CMS.MVC.Services;
using FitnessClub_Test.Core.Helpers;
using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FitnessClub_Test.CMS.MVC.Controllers
{
    [Authorize]
    public class PremadeProgramsController : BaseController
    {
        private readonly ApiAuthClient _api;
        private readonly FitnessClubDbContext _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly ILogger<PremadeProgramsController> _logger;
        

        public PremadeProgramsController(ApiAuthClient api, ILogger<PremadeProgramsController> logger
            ,IFileUploadService fileUploadService, FitnessClubDbContext context) : base(api)
        {
            _api = api;
            _logger = logger;
            _context = context;
            _fileUploadService = fileUploadService;
        }
        
        public async Task<IActionResult> Index()
        {
            try
            {
                var client = await _api.GetAuthorizedClientAsync();
                var response = await client.GetFromJsonAsync<List<PremadeProgramsDTO>>("premade-programs/all");

                return View("Index",response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Couldn't load all the progams");
                return View(new List<PremadeProgramsDTO>());
            }
        }

        [Authorize(Roles = "Admin, Coach")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _api.GetAuthorizedClientAsync();
            var response = await client.PutAsync($"premade-programs/delete/{id}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Program deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Error deleting program.";
            }

            return RedirectToAction("Index");
        }

        // returns view of the details 
        public async Task<IActionResult> Details(int programId)
        {
            var client = await _api.GetAuthorizedClientAsync();
            var program = await client.GetFromJsonAsync<PremadeProgramsDTO>($"premade-programs/details/{programId}");

            if (program == null)
                return NotFound();

            var responseExercises = await client.GetAsync("exercise");

            if (responseExercises.IsSuccessStatusCode)
            {
                var exercises = await responseExercises.Content.ReadFromJsonAsync<List<ExerciseDTO>>();
                ViewBag.Exercises = exercises;
            }
            else
            {
                ViewBag.Exercises = new List<ExerciseDTO>();
            }

            await PopulateDropdowns();
            return View(program);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Coach")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePremadeProgram(PremadeProgramsDTO dto)
        {
            var client = await _api.GetAuthorizedClientAsync();

            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Key: {entry.Key}, Error: {error.ErrorMessage}");
                    }
                }
                await PopulateDropdowns();
                
                var program = await client.GetFromJsonAsync<PremadeProgramsDTO>($"premade-programs/details/{dto.ID}");
                TempData["Error"] = "Program Could Not be Updated";
                return View("Details", program); 
            }

            // Handle Image 
            if (!string.IsNullOrEmpty(dto.CoverImageBase64))
            {
                byte[] imageBytes;
                var token = HttpContext.Session.GetString("access");
                JwtHelper.GetUserID(token, out int UserID);

                Console.WriteLine($"UserID: {UserID}");

                try
                {
                    imageBytes = Convert.FromBase64String(dto.CoverImageBase64);
                    Console.WriteLine($"imageBytes: {imageBytes.Length}");
                }
                catch
                {
                    Console.WriteLine($"INSIDE CATCH: Invalid image data.");
                    ModelState.AddModelError("Photo", "Invalid image data.");
                    return View("ProgramCreateView", dto);

                }

                // 1️. Size
                if (!ImageProcessingHelper.IsWithinSizeLimit(imageBytes, 3 * 1024 * 1024))
                {
                    ModelState.AddModelError("Photo", "Image must be 3 MB or smaller.");
                    return View("ProgramCreateView", dto);
                }

                // 2️. Extension
                if (!ImageProcessingHelper.IsAllowedExtension(dto.CoverImageName))
                {
                    ModelState.AddModelError("Photo", "Only JPG, JPEG, or PNG images are allowed.");
                    return View("ProgramCreateView", dto);
                }

                // 3️.Format
                if (!ImageProcessingHelper.IsValidImageFormat(imageBytes))
                {
                    ModelState.AddModelError("Photo", "Invalid image format.");
                    return View("ProgramCreateView", dto);
                }

                // 4️. Resize
                var extension = Path.GetExtension(dto.CoverImageName).ToLowerInvariant();

                imageBytes = ImageProcessingHelper.ResizeAndCompress(imageBytes, maxWidth: 512, maxHeight: 512, extension);
                Console.WriteLine($"imageBytes Resized: {imageBytes.Length}");

                string fileName = dto.CoverImageName ?? $"profile_{UserID}.png";
                Console.WriteLine($"fileName: {fileName}");

                var relativePath = await _fileUploadService.UploadFileAsync(imageBytes, fileName);
                Console.WriteLine($"relativePath: {relativePath}");

                dto.CoverImage = relativePath;
                Console.WriteLine($"before save");

                var program = await _context.PremadePrograms.FirstOrDefaultAsync(x => x.Id == dto.ID);

                if (program != null)
                {
                    program.CoverImage = relativePath;
                    await _context.SaveChangesAsync();
                }
                Console.WriteLine($"after save");
            }
            var response = await client.PostAsJsonAsync("premade-programs/save", dto);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error Updating Program";
                await PopulateDropdowns();
                return RedirectToAction("Details", dto.ID);
            }
               
            TempData["Success"] = "Information Saved Successfully";
            return RedirectToAction("Index");
        }


        // post action of create
        [Authorize(Roles = "Admin, Coach")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProgram(PremadeProgramsDTO dto)
        {
            var client = await _api.GetAuthorizedClientAsync();

            var coaches = await _context.Coaches
                .Include(c => c.User)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.User.FirstName + " " + c.User.LastName
                })
                .ToListAsync();
            var Exerciseresponse = await client.GetAsync("exercise");

            if (Exerciseresponse.IsSuccessStatusCode)
            {
                var exercises = await Exerciseresponse.Content.ReadFromJsonAsync<List<ExerciseDTO>>();
                ViewBag.Exercises = exercises;
            }
            else
            {
                ViewBag.Exercises = new List<ExerciseDTO>();
            }
            ViewBag.Coaches = coaches;
            await PopulateDropdowns();

            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Key: {entry.Key}");
                        Console.WriteLine($"Error: {error.ErrorMessage}");
                        Console.WriteLine($"Exception: {error.Exception}");
                        Console.WriteLine("-----------------------------");
                    }
                }
                return View("ProgramCreateView", dto);
            }

            // Handle Image 
            if (!string.IsNullOrEmpty(dto.CoverImageBase64))
            {
                byte[] imageBytes;
                var token = HttpContext.Session.GetString("access");
                JwtHelper.GetUserID(token, out int UserID);

                Console.WriteLine($"UserID: {UserID}");

                try
                {
                    imageBytes = Convert.FromBase64String(dto.CoverImageBase64);
                    Console.WriteLine($"imageBytes: {imageBytes.Length}");
                }
                catch
                {
                    Console.WriteLine($"INSIDE CATCH: Invalid image data.");
                    ModelState.AddModelError("Photo", "Invalid image data.");
                    return View("ProgramCreateView", dto);

                }

                // 1️. Size
                if (!ImageProcessingHelper.IsWithinSizeLimit(imageBytes, 3 * 1024 * 1024))
                {
                    ModelState.AddModelError("Photo", "Image must be 3 MB or smaller.");
                    return View("ProgramCreateView", dto);
                }

                // 2️. Extension
                if (!ImageProcessingHelper.IsAllowedExtension(dto.CoverImageName))
                {
                    ModelState.AddModelError("Photo", "Only JPG, JPEG, or PNG images are allowed.");
                    return View("ProgramCreateView", dto);
                }

                // 3️.Format
                if (!ImageProcessingHelper.IsValidImageFormat(imageBytes))
                {
                    ModelState.AddModelError("Photo", "Invalid image format.");
                    return View("ProgramCreateView", dto);
                }

                // 4️. Resize
                var extension = Path.GetExtension(dto.CoverImageName).ToLowerInvariant();

                imageBytes = ImageProcessingHelper.ResizeAndCompress(imageBytes, maxWidth: 512, maxHeight: 512, extension);
                Console.WriteLine($"imageBytes Resized: {imageBytes.Length}");

                string fileName = dto.CoverImageName ?? $"profile_{UserID}.png";
                Console.WriteLine($"fileName: {fileName}");

                var relativePath = await _fileUploadService.UploadFileAsync(imageBytes, fileName);
                Console.WriteLine($"relativePath: {relativePath}");

                dto.CoverImage = relativePath;
                Console.WriteLine($"before save");

                await _context.SaveChangesAsync();
                Console.WriteLine($"after save");
            }


            var response = await client.PostAsJsonAsync("premade-programs/create", dto);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error saving program.");
                return View("ProgramCreateView", dto);
            }

            // Redirect on success
            TempData["Success"] = "Program created successfully.";
            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult CreateExercises()
        {
            return View();
        }

        private async Task PopulateDropdowns()
        {
            ViewBag.LevelOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Easy", Value = "Easy" },
                new SelectListItem { Text = "Intermediate", Value = "Intermediate" },
                new SelectListItem { Text = "Hard", Value = "Hard" }
            };
            
            ViewBag.IntensityOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Low", Value = "Low" },
                new SelectListItem { Text = "Mid", Value = "Mid" },
                new SelectListItem { Text = "High", Value = "High" }
            };

            var coaches = await _context.Coaches
                .Include(c => c.User)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.User.FirstName + " " + c.User.LastName
                })
                .ToListAsync();

            ViewBag.Coaches = coaches;
        }
        
        
        // returns to create view
        [Authorize(Roles = "Admin, Coach")]
        public async Task<IActionResult> Create()
        {
            var client = await _api.GetAuthorizedClientAsync();
            await PopulateDropdowns();
            var response = await client.GetAsync("exercise");

            if (response.IsSuccessStatusCode)
            {
                var exercises = await response.Content.ReadFromJsonAsync<List<ExerciseDTO>>();
                ViewBag.Exercises = exercises;
            }
            else
            {
                ViewBag.Exercises = new List<ExerciseDTO>();
            }

            var coaches = await _context.Coaches
                .Include(c => c.User)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.User.FirstName + " " + c.User.LastName
                })
                .ToListAsync();

            ViewBag.Coaches = coaches;


            return View("ProgramCreateView");
        }

    }
}
