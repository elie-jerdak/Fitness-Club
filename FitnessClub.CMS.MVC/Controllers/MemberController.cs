using FitnessClub_Test.CMS.MVC.Services;
using FitnessClub_Test.Core.Helpers;
using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FitnessClub.CMS.MVC.Controllers
{
    //[Authorize]
    public class MemberController : BaseController
    {
        
        private readonly ApiAuthClient _api;
        private readonly string _apibaseUrl;
        private readonly IFileUploadService _fileUploadService;
        private readonly FitnessClubDbContext _context;

        public MemberController(ApiAuthClient api, FitnessClubDbContext context,
            IConfiguration configuration, IFileUploadService fileUploadService) : base(api)
        {
            _api = api;
            _apibaseUrl = configuration["Urls:ApiBaseUrl"];
            _context = context;
            _fileUploadService = fileUploadService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var client = await _api.GetAuthorizedClientAsync();

            var response = await client.GetAsync($"{_apibaseUrl}members/get-members");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var members = JsonConvert.DeserializeObject<List<MemberDTO>>(json);

            return View("Members", members);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int memberId)
        {
            var client = await _api.GetAuthorizedClientAsync();

            var response = await client.GetAsync($"{_apibaseUrl}members/get-member-for-update/{memberId}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var member = JsonConvert.DeserializeObject<MemberEditDTO>(json);

            PopulateDropdowns();
            return View("MemberEditView", member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost([FromBody] MemberEditDTO dto)
        {
            if (dto == null)
                return BadRequest(new
                {
                    errors = new Dictionary<string, string[]>
                    {
                        { "_", new[] { "Invalid request" } }
                    }
                });

            try
            {
                var client = await _api.GetAuthorizedClientAsync();
                var content = JsonContent.Create(dto);

                // Handle Image 
                if (!string.IsNullOrEmpty(dto.ProfileImageBase64))
                {
                    byte[] imageBytes;
                    var token = HttpContext.Session.GetString("access");
                    JwtHelper.GetUserID(token, out int UserID);

                    Console.WriteLine($"UserID: {UserID}");

                    try
                    {
                        imageBytes = Convert.FromBase64String(dto.ProfileImageBase64);
                        Console.WriteLine($"imageBytes: {imageBytes.Length}");
                    }
                    catch
                    {
                        Console.WriteLine($"INSIDE CATCH: Invalid image data.");
                        ModelState.AddModelError("Photo", "Invalid image data.");
                        return View(dto);

                    }

                    // 1️. Size
                    if (!ImageProcessingHelper.IsWithinSizeLimit(imageBytes, 3 * 1024 * 1024))
                    {
                        ModelState.AddModelError("Photo", "Image must be 3 MB or smaller.");
                        return View(dto);
                    }

                    // 2️. Extension
                    if (!ImageProcessingHelper.IsAllowedExtension(dto.ProfileImageFileName))
                    {
                        ModelState.AddModelError("Photo", "Only JPG, JPEG, or PNG images are allowed.");
                        return View(dto);
                    }

                    // 3️.Format
                    if (!ImageProcessingHelper.IsValidImageFormat(imageBytes))
                    {
                        ModelState.AddModelError("Photo", "Invalid image format.");
                        return View(dto);
                    }

                    // 4️. Resize
                    var extension = Path.GetExtension(dto.ProfileImageFileName).ToLowerInvariant();

                    imageBytes = ImageProcessingHelper.ResizeAndCompress(imageBytes, maxWidth: 512, maxHeight: 512, extension);
                    Console.WriteLine($"imageBytes Resized: {imageBytes.Length}");

                    string fileName = dto.ProfileImageFileName ?? $"profile_{UserID}.png";
                    Console.WriteLine($"fileName: {fileName}");

                    var relativePath = await _fileUploadService.UploadFileAsync(imageBytes, fileName);
                    Console.WriteLine($"relativePath: {relativePath}");

                    dto.Photo = relativePath;
                    Console.WriteLine($"before save");

                    await _context.SaveChangesAsync();
                    Console.WriteLine($"after save");
                }

                var response = await client.PostAsync($"members/edit-member/{dto.MemberID}", content);

                // Only deserialize errors when request FAILED
                if (!response.IsSuccessStatusCode)
                {
                    ApiErrorResponse wrapper = null;

                    if (response.Content.Headers.ContentLength > 0)
                    {
                        wrapper = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                    }

                    return StatusCode(
                        (int)response.StatusCode,
                        new
                        {
                            errors = wrapper?.Errors ?? 
                                new Dictionary<string, string[]>
                                {
                                   { "_", new[] { "Request failed." } }
                                }
                        }
                    );
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    errors = new Dictionary<string, string[]>
            {
                { "_", new[] { "Unexpected error occurred." } }
            }
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int MemberID)
        {
            var client = await _api.GetAuthorizedClientAsync();
            var response = await client.GetAsync($"{_apibaseUrl}members/get-member-for-delete/{MemberID}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var member = JsonConvert.DeserializeObject<MemberDTO>(json);

            return View("MemberDeleteView", member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int MemberID)
        {
            if (MemberID <= 0)
            {
                ModelState.AddModelError("", "Invalid member.");
                return RedirectToAction("Index");
            }

            var client = await _api.GetAuthorizedClientAsync();
            var response = await client.DeleteAsync($"{_apibaseUrl}members/delete-member/{MemberID}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Failed to delete member.");
            return RedirectToAction("Delete", new { MemberID });
        }

        [HttpPost]
        public async Task<IActionResult> BulkDelete(List<int> selectedMemberIDs)
        {
            if (selectedMemberIDs == null || !selectedMemberIDs.Any())
            {
                TempData["Error"] = "No members selected.";
                return RedirectToAction("Index");
            }

            var client = await _api.GetAuthorizedClientAsync();
            var response = await client.PostAsJsonAsync($"{_apibaseUrl}members/bulk-delete-members", selectedMemberIDs);

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

        [HttpGet]
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View("CreateMemberView");
        }

        [HttpPost]
        public async Task<ActionResult> CreateConfirmed([FromBody] FullMemberDto fullMemberDto)
        {
            Console.WriteLine("Inside MVC Controller");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model State is Invalid");
                PopulateDropdowns();
                return View("CreateMemberView", fullMemberDto);
            }

            // Handle Image 
            if (!string.IsNullOrEmpty(fullMemberDto.ProfileImageBase64))
            {
                byte[] imageBytes;
                var token = HttpContext.Session.GetString("access");
                JwtHelper.GetUserID(token, out int UserID);
                
                Console.WriteLine($"UserID: {UserID}");

                try
                {
                    imageBytes = Convert.FromBase64String(fullMemberDto.ProfileImageBase64);
                    Console.WriteLine($"imageBytes: {imageBytes.Length}");
                }
                catch
                {
                    Console.WriteLine($"INSIDE CATCH: Invalid image data.");
                    ModelState.AddModelError("Photo", "Invalid image data.");
                    return View(fullMemberDto);

                }

                // 1️. Size
                if (!ImageProcessingHelper.IsWithinSizeLimit(imageBytes, 3 * 1024 * 1024))
                {
                    ModelState.AddModelError("Photo", "Image must be 3 MB or smaller.");
                    return View(fullMemberDto);
                }

                // 2️. Extension
                if (!ImageProcessingHelper.IsAllowedExtension(fullMemberDto.ProfileImageFileName))
                {
                    ModelState.AddModelError("Photo", "Only JPG, JPEG, or PNG images are allowed.");
                    return View(fullMemberDto);
                }

                // 3️.Format
                if (!ImageProcessingHelper.IsValidImageFormat(imageBytes))
                {
                    ModelState.AddModelError("Photo", "Invalid image format.");
                    return View(fullMemberDto);
                }

                // 4️. Resize
                var extension = Path.GetExtension(fullMemberDto.ProfileImageFileName).ToLowerInvariant();

                imageBytes = ImageProcessingHelper.ResizeAndCompress(imageBytes, maxWidth: 512, maxHeight: 512, extension);
                Console.WriteLine($"imageBytes Resized: {imageBytes.Length}");

                string fileName = fullMemberDto.ProfileImageFileName ?? $"profile_{UserID}.png";
                Console.WriteLine($"fileName: {fileName}");

                var relativePath = await _fileUploadService.UploadFileAsync(imageBytes, fileName);
                Console.WriteLine($"relativePath: {relativePath}");

                fullMemberDto.user.Photo = relativePath;
                Console.WriteLine($"before save");

                await _context.SaveChangesAsync();
                Console.WriteLine($"after save");
            }

            var client = await _api.GetAuthorizedClientAsync();
            
            Console.WriteLine("Before going to api");
            var response = await client.PostAsJsonAsync($"{_apibaseUrl}members/create-full-member", fullMemberDto);
            Console.WriteLine("After going to api");
            
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            // Read API validation errors (if any)
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", error);

            PopulateDropdowns();
            return View("CreateMemberView", fullMemberDto);
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PremadeProgramsDTO>>> GetProgramsBoughtByUser(int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid user ID.");

            var client = await _api.GetAuthorizedClientAsync();
            var response = await client.GetAsync($"{_apibaseUrl}members/get-purchased-programs/{userId}");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to fetch purchased programs.");

            var programs = await response.Content.ReadFromJsonAsync<List<PremadeProgramsDTO>>();

            return View(programs);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid user ID");

            var client = await _api.GetAuthorizedClientAsync(); 
            var response = await client.GetAsync($"members/get-member-details/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound();

                return StatusCode((int)response.StatusCode, "Error fetching member details");
            }

            var vm = await response.Content.ReadFromJsonAsync<MemberDetailViewDTO>();

            if (vm == null)
                return NotFound();

            PopulateDropdowns();
            return View("Details", vm);
        }

        [HttpGet]
        public async Task<IActionResult> GetQrByID(int UserID)
        {
            var client = await _api.GetAuthorizedClientAsync();
            var response = await client.GetAsync($"members/get-qr-by-id/{UserID}");

            if (!response.IsSuccessStatusCode)
                return Json(new { success = false });

            var json = await response.Content.ReadAsStringAsync();
            var qrToken = JObject.Parse(json)["qrToken"]?.ToString();

            using var gen = new QRCodeGenerator();
            var data = gen.CreateQrCode(qrToken, QRCodeGenerator.ECCLevel.Q);
            var qrBytes = new PngByteQRCode(data).GetGraphic(20);

            return Json(new
            {
                success = true,
                qrCodeBase64 = $"data:image/png;base64,{Convert.ToBase64String(qrBytes)}",
                qrToken
            });
        }


        private void PopulateDropdowns()
        {
            ViewBag.GenderOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Male", Value = "M" },
                new SelectListItem { Text = "Female", Value = "F" }
            };
            
            ViewBag.ClientTarget = new List<SelectListItem>
            {
                new SelectListItem { Text = "Lose Weight", Value = "Lose Weight" },
                new SelectListItem { Text = "Build Muscle", Value = "Build Muscle" },
                new SelectListItem { Text = "Endurance", Value = "Endurance" }
            };
            
            ViewBag.MembershipType = new List<SelectListItem>
            {
                new SelectListItem { Text = "Trial", Value = "Trial" },
                new SelectListItem { Text = "Monthly", Value = "Monthly" },
                new SelectListItem { Text = "Yearly", Value = "Yearly" }
            };
            
            ViewBag.UserRole = new List<SelectListItem>
            {
                new SelectListItem { Text = "Client", Value = "Client" },
                new SelectListItem { Text = "Admin", Value = "Admin" },
                new SelectListItem { Text = "Coach", Value = "Coach" }
            };
        }

    }
}