using FitnessClub_Test.CMS.MVC.Services;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace FitnessClub_Test.CMS.MVC.Controllers
{
    public class ExerciseController : BaseController
    {
        private readonly ApiAuthClient _client;

        public ExerciseController(ApiAuthClient client) : base(client)
        {
            _client = client;
        }

        // INDEX
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var client = await _client.GetAuthorizedClientAsync();

            var response = await client.GetAsync("exercise");
            if (!response.IsSuccessStatusCode)
                return View(new List<ExerciseDTO>());

            var exercises = await response.Content
                .ReadFromJsonAsync<List<ExerciseDTO>>();

            return View(exercises);
        }

        // CREATE (POST)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ExerciseDTO dto)
        {
            if (dto == null)
                return BadRequest(new { message = "No exercise data received. Make sure Content-Type is application/json." });

            if (!ModelState.IsValid)
                return BadRequest(new { errors = ModelState });

            var client = await _client.GetAuthorizedClientAsync();

            try
            {
                // Send to API
                var response = await client.PostAsJsonAsync("exercise/create", dto);

                // Check if API responded with success
                if (!response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    object errorData = null;
                    try { errorData = JsonSerializer.Deserialize<object>(responseContent); } catch { errorData = responseContent; }

                    return BadRequest(new { message = "Failed to create exercise.", apiResponse = errorData });
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception is: {ex.Message}");
                // Catch unexpected exceptions (network, deserialization, etc.)
                return StatusCode(500, new { message = "Unexpected error.", details = ex.Message });
            }
        }

        // EDIT (POST)
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] ExerciseDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Invalid data",
                    errors = ModelState
                });
            }

            var client = await _client.GetAuthorizedClientAsync();

            var response = await client.PutAsJsonAsync("exercise/update", dto);

            var content = await response.Content.ReadAsStringAsync();

            // Forward API response EXACTLY
            return StatusCode(
                (int)response.StatusCode,
                string.IsNullOrEmpty(content)
                    ? null
                    : System.Text.Json.JsonSerializer.Deserialize<object>(content)
            );
        }

        // DELETE (Delete – soft delete)
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if(id <= 0)
            {
                return BadRequest("Exercise Not Found");
            }
            
            var client = await _client.GetAuthorizedClientAsync();
            var response = await client.DeleteAsync($"exercise/delete/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return BadRequest(!string.IsNullOrWhiteSpace(content) ? content : "Failed to Delete the Exercise");
            }

            return Ok("Exercise Deleted Successfully");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BulkDelete([FromForm] int[] selectedIds)
        {
            if (selectedIds == null || selectedIds.Length == 0)
            {
                ModelState.AddModelError("", "No exercises selected for deletion.");
                return RedirectToAction(nameof(Index));
            }

            var client = await _client.GetAuthorizedClientAsync();

            // Call API, passing IDs as JSON
            var response = await client.PostAsJsonAsync("exercise/bulk-delete", selectedIds);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to delete exercises.");
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}