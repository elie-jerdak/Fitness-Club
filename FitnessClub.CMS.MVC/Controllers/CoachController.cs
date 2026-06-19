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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.CMS.MVC.Controllers
{
    public class CoachController : BaseController
    {
        private readonly ApiAuthClient _api;
        private readonly ILogger<CoachController> _logger;
        private readonly ICoachService _coachService;
        private readonly IFileUploadService _fileUploadService;
        private readonly FitnessClubDbContext _context;
        public CoachController(ApiAuthClient api, ILogger<CoachController> logger, ICoachService service,
            IFileUploadService fileUploadService, FitnessClubDbContext context) : base(api)
        {
            _api = api;
            _logger = logger;
            _coachService = service;
            _fileUploadService = fileUploadService;
            _context = context;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                var client = await _api.GetAuthorizedClientAsync();
                var response = await client.GetAsync("coach");

                if (response.IsSuccessStatusCode)
                {
                    var coaches = await response.Content
                        .ReadFromJsonAsync<List<CoachDTO>>();

                    return View(coaches ?? new List<CoachDTO>());
                }

                // Handle known API errors
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["Info"] = "No coaches available at the moment.";
                    return View(new List<CoachDTO>());
                }

                // Other errors
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("API error fetching coaches: {Error}", error);

                TempData["Error"] = "Unable to load coaches.";
                return View(new List<CoachDTO>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load coaches");
                TempData["Error"] = "An unexpected error occurred.";
                return View(new List<CoachDTO>());
            }
        }

        [Authorize(Roles = "Admin, Coach")]
        [HttpGet]
        public async Task<IActionResult> Edit(int UserID)
        {
            var client = await _api.GetAuthorizedClientAsync();
            var response = await client.GetAsync($"coach/edit/{UserID}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var coach = JsonConvert.DeserializeObject<CoachEditDTO>(json);

            PopulateDropdowns();
            return View("CoachEditView", coach);
        }

        [Authorize(Roles = "Admin, Coach")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromBody] CoachEditDTO Dto)
        {
            Console.WriteLine("inside update controller");
            Console.WriteLine($"this is the dto userID: {Dto.UserID}");
            Console.WriteLine($"this is the dto qrCode insdide the mvc: {Dto.QrCode}");

            // Handle Image 
            if (!string.IsNullOrEmpty(Dto.ProfileImageBase64))
            {
                byte[] imageBytes;
                var token = HttpContext.Session.GetString("access");
                JwtHelper.GetUserID(token, out int UserID);

                Console.WriteLine($"UserID: {UserID}");

                try
                {
                    imageBytes = Convert.FromBase64String(Dto.ProfileImageBase64);
                    Console.WriteLine($"imageBytes: {imageBytes.Length}");
                }
                catch
                {
                    Console.WriteLine($"INSIDE CATCH: Invalid image data.");
                    ModelState.AddModelError("Photo", "Invalid image data.");
                    return View(Dto);

                }

                // 1️. Size
                if (!ImageProcessingHelper.IsWithinSizeLimit(imageBytes, 3 * 1024 * 1024))
                {
                    ModelState.AddModelError("Photo", "Image must be 3 MB or smaller.");
                    return View(Dto);
                }

                // 2️. Extension
                if (!ImageProcessingHelper.IsAllowedExtension(Dto.ProfileImageFileName))
                {
                    ModelState.AddModelError("Photo", "Only JPG, JPEG, or PNG images are allowed.");
                    return View(Dto);
                }

                // 3️.Format
                if (!ImageProcessingHelper.IsValidImageFormat(imageBytes))
                {
                    ModelState.AddModelError("Photo", "Invalid image format.");
                    return View(Dto);
                }

                // 4️. Resize
                var extension = Path.GetExtension(Dto.ProfileImageFileName).ToLowerInvariant();

                imageBytes = ImageProcessingHelper.ResizeAndCompress(imageBytes, maxWidth: 512, maxHeight: 512, extension);
                Console.WriteLine($"imageBytes Resized: {imageBytes.Length}");

                string fileName = Dto.ProfileImageFileName ?? $"profile_{UserID}.png";
                Console.WriteLine($"fileName: {fileName}");

                var relativePath = await _fileUploadService.UploadFileAsync(imageBytes, fileName);
                Console.WriteLine($"relativePath: {relativePath}");

                Dto.Photo = relativePath;
                Console.WriteLine($"before save");

                await _context.SaveChangesAsync();
                Console.WriteLine($"after save");
            }

            var client = await _api.GetAuthorizedClientAsync();
            var content = new StringContent(JsonConvert.SerializeObject(Dto), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("coach/updateCoach", content);

            Console.WriteLine("after api response");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"this is the status code that was returned from the api: {response.StatusCode}");
                return RedirectToAction("Index");
            }

            Console.WriteLine($"this is the status code that was returned from the api: {response.StatusCode}");
            ModelState.AddModelError(string.Empty, "Failed to update member.");
            return View("CoachEditView", Dto);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int UserID)
        {
            var client = await _api.GetAuthorizedClientAsync();
            var response = await client.GetAsync($"coach/delete/{UserID}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var coach = JsonConvert.DeserializeObject<CoachDTO>(json);

            return View("CoachDeleteView", coach);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ConfirmDelete(int UserID)
        {
            var client = await _api.GetAuthorizedClientAsync();
            var response = await client.DeleteAsync($"coach/ConfirmDelete/{UserID}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ViewBag.Error = error;

                return NotFound();
            }
            TempData["Success"] = "Coach deleted successfully!";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> BulkDelete([FromForm] List<int> selectedCoachIds)
        {
            if (selectedCoachIds == null || !selectedCoachIds.Any())
            {
                TempData["Error"] = "No members selected.";
                return RedirectToAction("Index");
            }


            var client = await _api.GetAuthorizedClientAsync();

            var content = new StringContent(JsonConvert.SerializeObject(selectedCoachIds), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("coach/bulk-delete", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<BulkDeleteDTO>();

                if (result.FailedCount == 0)
                {
                    TempData["Success"] = result.Message;
                }
                else
                {
                    TempData["Warning"] =
                    $"{result.Message} Deleted: {result.DeletedCount}, Failed: {result.FailedCount}.";
                }

                return RedirectToAction("Index");
            }

            TempData["Error"] = "Failed to delete selected members.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            var coachCreateDto = new CoachCreateDTO();
            PopulateDropdowns();
            return View("CreateCoachView", coachCreateDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCoach([FromBody] CoachCreateDTO coachCreateDTO)
        {
            if (coachCreateDTO == null)
            {
                return BadRequest(new
                {
                    errors = new Dictionary<string, string[]>
                    {
                        { "_", new[] { "Invalid request." } }
                    }
                });
            }

            // Handle Image 
            if (!string.IsNullOrEmpty(coachCreateDTO.ProfileImageBase64))
            {
                byte[] imageBytes;
                var token = HttpContext.Session.GetString("access");
                JwtHelper.GetUserID(token, out int UserID);

                Console.WriteLine($"UserID: {UserID}");

                try
                {
                    imageBytes = Convert.FromBase64String(coachCreateDTO.ProfileImageBase64);
                    Console.WriteLine($"imageBytes: {imageBytes.Length}");
                }
                catch
                {
                    Console.WriteLine($"INSIDE CATCH: Invalid image data.");
                    ModelState.AddModelError("Photo", "Invalid image data.");
                    return View(coachCreateDTO);

                }

                // 1️. Size
                if (!ImageProcessingHelper.IsWithinSizeLimit(imageBytes, 3 * 1024 * 1024))
                {
                    ModelState.AddModelError("Photo", "Image must be 3 MB or smaller.");
                    return View(coachCreateDTO);
                }

                // 2️. Extension
                if (!ImageProcessingHelper.IsAllowedExtension(coachCreateDTO.ProfileImageFileName))
                {
                    ModelState.AddModelError("Photo", "Only JPG, JPEG, or PNG images are allowed.");
                    return View(coachCreateDTO);
                }

                // 3️.Format
                if (!ImageProcessingHelper.IsValidImageFormat(imageBytes))
                {
                    ModelState.AddModelError("Photo", "Invalid image format.");
                    return View(coachCreateDTO);
                }

                // 4️. Resize
                var extension = Path.GetExtension(coachCreateDTO.ProfileImageFileName).ToLowerInvariant();

                imageBytes = ImageProcessingHelper.ResizeAndCompress(imageBytes, maxWidth: 512, maxHeight: 512, extension);
                Console.WriteLine($"imageBytes Resized: {imageBytes.Length}");

                string fileName = coachCreateDTO.ProfileImageFileName ?? $"profile_{UserID}.png";
                Console.WriteLine($"fileName: {fileName}");

                var relativePath = await _fileUploadService.UploadFileAsync(imageBytes, fileName);
                Console.WriteLine($"relativePath: {relativePath}");

                coachCreateDTO.Photo = relativePath;
                Console.WriteLine($"before save");

                await _context.SaveChangesAsync();
                Console.WriteLine($"after save");
            }

            var client = await _api.GetAuthorizedClientAsync();
            var response = await client.PostAsJsonAsync("coach/ConfirmCreate", coachCreateDTO);

            if (!response.IsSuccessStatusCode)
            {
                // Forward API errors as-is
                var apiError = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, apiError);
            }

            return Ok();
        }

        [Authorize(Roles = "Admin, Coach")]
        [HttpGet]
        public async Task<IActionResult> Details(int userId)
        {
            var client = await _api.GetAuthorizedClientAsync();
            var response = await client.GetAsync($"coach/details/{userId}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var coachDto = await response.Content.ReadFromJsonAsync<FullCoachDTO>();

            var clients = await _context.Clients
                .Include(c => c.User)
                .Select(c => new
                {
                    c.Id,
                    FullName = c.User.FirstName + " " + c.User.LastName
                })
                .ToListAsync();

            ViewBag.Clients = clients;

            // Pass the DTO to the view to fill the form
            return View("CoachDetailsView", coachDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> FreeTimeSlot(int AvailabilityId, int ClientId, int CoachUserId)
        {
            var client = await _api.GetAuthorizedClientAsync();

            var response = await client.PostAsJsonAsync("coach/freeslot", new
            {
                AvailabilityId,
                ClientId
            });

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Time Slot Couldn't be Freed";
                return NotFound();
            }

            TempData["Success"] = "Time Slot Freed Successfully";
            return RedirectToAction("Details", new { userId = CoachUserId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ReserveTimeSlot(int AvailabilityId, int ClientId, int CoachUserID)
        {
            // Get authorized HTTP client
            var client = await _api.GetAuthorizedClientAsync();

            // Call API
            var response = await client.PostAsJsonAsync("coach/reserve", new
            {
                AvailabilityId,
                ClientId
            });

            // If API request failed (network/server error)
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Time Slot Couldn't be Reserved due to a server error.";
                return RedirectToAction("Details", new { userId = CoachUserID }); // fallback redirect
            }

            // Read API response
            var apiResult = await response.Content.ReadFromJsonAsync<ReserveSlotResponse>();

            if (apiResult == null)
            {
                TempData["Error"] = "Unexpected response from server.";
                return RedirectToAction("Details", new { userId = CoachUserID });
            }

            // Set TempData based on success/failure
            if (apiResult.Success)
                TempData["Success"] = apiResult.Message;
            else
                TempData["Error"] = apiResult.Message;

            // Redirect using the CoachUserId provided by API
            return RedirectToAction("Details", new { userId = CoachUserID });
        }

        [Authorize(Roles = "Admin, Coach")]
        [HttpGet]
        public IActionResult CreateAvailability(int CoachUserId)
        {
            var now = DateTime.Now;

            var vm = new CreateAvailabilityViewModel
            {
                CoachUserId = CoachUserId,
                Day = DateOnly.FromDateTime(now),
                StartTime = TimeOnly.FromDateTime(now),
                EndTime = TimeOnly.FromDateTime(now.AddHours(1))
            };

            return View(vm);
        }

        [Authorize(Roles = "Admin, Coach")]
        [HttpPost]
        public async Task<IActionResult> CreateAvailability(CreateAvailabilityViewModel dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var client = await _api.GetAuthorizedClientAsync();

            var response = await client.PostAsJsonAsync("coach/createAvailability", dto);

            if (!response.IsSuccessStatusCode)
            {
                // Read the error message returned by the API
                string apiMessage = await response.Content.ReadAsStringAsync();
                TempData["Error"] = !string.IsNullOrEmpty(apiMessage)
                    ? apiMessage
                    : "Error creating availability";

                return RedirectToAction("CreateAvailability",
                    new { dto.CoachUserId });
            }


            TempData["Success"] = "Availability Created Successfully";

            return RedirectToAction("Details",
                new { userId = dto.CoachUserId });
        }


        [Authorize(Roles = "Admin, Coach")]
        [HttpGet]
        public async Task<IActionResult> EditAvailability(int id)
        {
            var client = await _api.GetAuthorizedClientAsync();
            var res = await client.GetAsync($"coach/availability/{id}");
            if (!res.IsSuccessStatusCode)
                return NotFound();

            var availability = await res.Content.ReadFromJsonAsync<AvailabilityDTO>();

            var model = new EditAvailabilityViewModel
            {
                CoachUserId = await _coachService.GetCoachUserIdByAvailabilityId(id),
                Availability = availability
            };

            return View("EditAvailabilityView", model);
        }

        [Authorize(Roles = "Admin, Coach")]
        [HttpPost]
        public async Task<IActionResult> PutAvailability(EditAvailabilityViewModel model)
        {
            if (!ModelState.IsValid)
                return View("EditAvailabilityView", model);

            var client = await _api.GetAuthorizedClientAsync();
            var res = await client.PutAsJsonAsync("coach/availability", model.Availability);

            if (!res.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error Updating Availability";
                return View("EditAvailabilityView", model); // redisplay with current data
            }

            TempData["Success"] = "Availability Updated Successfully";
            return RedirectToAction("Details", "Coach", new { userId = model.CoachUserId });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAvailability(int availabilityId, int coachUserId)
        {
            var client = await _api.GetAuthorizedClientAsync();
            var response = await client.DeleteAsync($"coach/deleteAvailability/{availabilityId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to delete availability.";
                return RedirectToAction("Details", new { userId = coachUserId }); 
            }

            TempData["Success"] = "Availability deleted successfully.";
            return RedirectToAction("Details", new { userId = coachUserId }); 
        }

        [Authorize]
        private void PopulateDropdowns()
        {
            ViewBag.GenderOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Male", Value = "M" },
                new SelectListItem { Text = "Female", Value = "F" }
            };

        }

    }
}
