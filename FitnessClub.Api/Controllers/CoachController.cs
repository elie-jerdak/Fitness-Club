using FitnessClub_Test.Core.Helpers;
using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.Services;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessClub_Test.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CoachController : ControllerBase
    {
        private readonly ICoachService _coachService;

        public CoachController(ICoachService coachService)
        {
            _coachService = coachService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCoaches()
        {
            try
            {
                var response = await _coachService.GetCoachesAsync();
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "An unexpected error occurred while fetching coaches."
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("edit/{UserID}")]
        public async Task<IActionResult> EditCoach(int UserID)
        {
            var response = await _coachService.EditCoach(UserID);

            if (response == null)
                return NotFound(new { message = $"Coach with User ID {UserID} not found." });

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("updateCoach")]
        public async Task<IActionResult> UpdateCoach(CoachEditDTO dto)
        {
            Console.WriteLine("inside api controllerrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr");
            Console.WriteLine($"this is the dto user id: {dto.UserID}");
            Console.WriteLine($"this is the dto qrCode: {dto.QrCode}");

            if (dto.UserID <= 0)
                return NotFound("Coach Not Found.");

            var updated = await _coachService.UpdateCoachAsync(dto);

            Console.WriteLine($"returning from service call with true/false: {updated}");

            if (!updated)
            {
                Console.WriteLine("Coach was not updated");
                return BadRequest(new { message = "Error updating coach." });
            }
              
            return Ok(new { message = "Coach updated successfully." });
        }

        [HttpGet("delete/{UserID}")]
        public async Task<IActionResult> DeleteCoachAsync(int UserID)
        {
            var response = await _coachService.DeleteCoachAsync(UserID);

            if (response == null)
                return NotFound(new { message = $"Coach with User ID {UserID} not found" });

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("ConfirmDelete/{UserID}")]
        public async Task<IActionResult> ConfirmDelete(int UserID)
        {
            var deleted = await _coachService.ConfirmDelete(UserID);

            if (!deleted)
                return NotFound(new { message = "Coach deletion failed." });

            return Ok(new { message = "Coach deleted successfully." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("bulk-delete")]
        public async Task<IActionResult> BulkDelete([FromBody] List<int> selectedCoachId)
        {
            if (selectedCoachId == null || !selectedCoachId.Any())
                return BadRequest("No user IDs provided.");

            var result = await _coachService.BulkDelete(selectedCoachId);

            if (result.FailedCount == 0)
            {
                return Ok(new
                {
                    message = "All coach records were deleted successfully.",
                    deletedCount = result.DeletedCount
                });
            }

            return Ok(new
            {
                message = "Some coach records could not be deleted.",
                deletedCount = result.DeletedCount,
                failedCount = result.FailedCount,
                failedUserIds = result.FailedUserIds
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("ConfirmCreate")]
        public async Task<IActionResult> ConfirmCreate(CoachCreateDTO dto)
        {
            Console.WriteLine("Inside Confirm Create Api");
            Console.WriteLine($"dto.Photo: {dto.Photo}");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Console.WriteLine("Passed Api model state check");
            var result = await _coachService.ConfirmCreate(dto);
            
            Console.WriteLine("came back from the service");
            
            if (!result.Success)
            {
                if (result.Errors != null)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error);
                }

                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("details/{userId}")]
        public async Task<ActionResult<FullCoachDTO>> Details(int userId)
        {
            var response = await _coachService.GetCoachDetails(userId);
            if (response == null)            
                return NotFound();
            
            return Ok(response);
        }

        [HttpPost("freeslot")]
        public async Task<IActionResult> FreeSlot([FromBody] TimeSlotDTO request)
        {
            var result = await _coachService.FreeTimeSlotAsync(request.AvailabilityId, request.ClientId);

            if (result)
                return Ok();

            return BadRequest("Slot not Found or Already Free");
        }

        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveSlot([FromBody] TimeSlotDTO dto)
        {
            var coachUserId = await _coachService.ReserveSlot(dto.AvailabilityId, dto.ClientId);

            var response = new ReserveSlotResponse
            {
                CoachUserId = coachUserId
            };

            switch (coachUserId)
            {
                case -1:
                    response.Success = false;
                    response.Message = "Availability or Client not found";
                    break;
                case -2:
                    response.Success = false;
                    response.Message = "You have already reserved this time slot";
                    break;
                case -3:
                    response.Success = false;
                    response.Message = "This time slot has already been reserved by another client";
                    break;
                default:
                    response.Success = true;
                    response.Message = "Time Slot Reserved Successfully";
                    break;
            }

            return Ok(response);
        }

        [Authorize(Roles = "Admin, Coach")]
        [HttpPost("createAvailability")]
        public async Task<IActionResult> CreateAvailability([FromBody] CreateAvailabilityViewModel dto)
        {
            var result = await _coachService.CreateAvailability(dto);

            if (result == -1)
                return NotFound("Coach not found");

            if (result == -2)
                return BadRequest("Time slot overlaps existing availability");

            return Ok(result); // return CoachUserId
        }

        [HttpGet("availability/{id}")]
        public async Task<ActionResult<AvailabilityDTO>> GetAvailabilityById(int id)
        {
            var dto = await _coachService.GetAvailabilityById(id);
            if (dto == null)
                return NotFound();

            return Ok(dto);
        }

        [Authorize(Roles = "Admin, Coach")]
        [HttpPut("availability")]
        public async Task<IActionResult> UpdateAvailability([FromBody] AvailabilityDTO dto)
        {
            var success = await _coachService.UpdateAvailability(dto);
            if (!success)
                return BadRequest("Update failed");

            return NoContent();
        }

        [HttpDelete("deleteAvailability/{availabilityId}")]
        public async Task<IActionResult> DeleteAvailability(int availabilityId)
        {
            var success = await _coachService.DeleteAvailabilityAsync(availabilityId);

            if (!success)
                return NotFound(new { message = "Availability not found." });

            return Ok(new { message = "Availability deleted successfully." });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-coach-user-id/{availabilityId}")]
        public async Task<ActionResult<int>> GetCoachUserIdByAvailabilityId(int availabilityId)
        {
            var coachUserId = await _coachService.GetCoachUserIdByAvailabilityId(availabilityId);
            if (coachUserId == 0)
                return NotFound();

            return Ok(coachUserId);
        }
    }
}
