using FitnessClub_Test.CMS.MVC.Services;
using FitnessClub_Test.Core.Helpers;
using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FitnessClub_Test.CMS.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminAccountController : BaseController
    {
        private readonly ApiAuthClient _api;
        
        private FitnessClubDbContext _context;
        private IFileUploadService _fileUploadService;

        private readonly ILogger<AdminAccountController> _logger;
        public AdminAccountController(ApiAuthClient api, ILogger<AdminAccountController> logger
            ,FitnessClubDbContext context, IFileUploadService fileUploadService) : base(api)
        {
            _api = api;
            _logger = logger;
            _context = context;
            _fileUploadService = fileUploadService;
        }

        // GET: AccountController
        public async Task<ActionResult> Index()
        {
            string token = HttpContext.Session.GetString("access");
            
            if (!JwtHelper.GetUserID(token, out int userId))
            {
                return RedirectToAction("Index", "Login"); // or return Unauthorized();
            }

            var client = await _api.GetAuthorizedClientAsync();
            var response = await client.GetAsync($"admin-account/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(new
                {
                    errors = new
                    {
                        _ = new[] { "User not found." }
                    }
                });
            }

            var adminDto = await response.Content.ReadFromJsonAsync<AdminAccountDTO>();

            return View("Account", adminDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin-account/edit")]
        public async Task<IActionResult> Edit([FromBody] AdminAccountDTO adminDto)
        {
            var token = HttpContext.Session.GetString("access");
            JwtHelper.GetUserID(token, out int UserID);

            var client = await _api.GetAuthorizedClientAsync();
            var content = JsonContent.Create(adminDto);
            
            try
            {
                // Handle Image 
                if (!string.IsNullOrEmpty(adminDto.ProfileImageBase64))
                {
                    byte[] imageBytes;

                    try
                    {
                        imageBytes = Convert.FromBase64String(adminDto.ProfileImageBase64);
                    }
                    catch
                    {
                        ModelState.AddModelError("Photo", "Invalid image data.");
                        return View(adminDto);

                    }

                    // 1️. Size
                    if (!ImageProcessingHelper.IsWithinSizeLimit(imageBytes, 3 * 1024 * 1024))
                    {
                        ModelState.AddModelError("Photo", "Image must be 3 MB or smaller.");
                        return View(adminDto);
                    }

                    // 2️. Extension
                    if (!ImageProcessingHelper.IsAllowedExtension(adminDto.ProfileImageFileName))
                    {
                        ModelState.AddModelError("Photo", "Only JPG, JPEG, or PNG images are allowed.");
                        return View(adminDto);
                    }

                    // 3️.Format
                    if (!ImageProcessingHelper.IsValidImageFormat(imageBytes))
                    {
                        ModelState.AddModelError("Photo", "Invalid image format.");
                        return View(adminDto);
                    }

                    // 4️. Resize
                    var extension = Path.GetExtension(adminDto.ProfileImageFileName).ToLowerInvariant();

                    imageBytes = ImageProcessingHelper.ResizeAndCompress(imageBytes, maxWidth: 512, maxHeight: 512, extension);

                    string fileName = adminDto.ProfileImageFileName ?? $"profile_{UserID}.png";

                    var relativePath = await _fileUploadService.UploadFileAsync(imageBytes, fileName);

                    adminDto.Photo = relativePath;

                    await _context.SaveChangesAsync();
                }

                var response = await client.PostAsync($"admin-account/editapi", content);

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var errors = await response.Content.ReadFromJsonAsync<Dictionary<string, string[]>>();
                    Console.WriteLine(errors);
                    return BadRequest(new { errors });
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return View("Forbidden");
                }

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Message: {ex}");
            }
            
            return Ok();
        }
    }
}
