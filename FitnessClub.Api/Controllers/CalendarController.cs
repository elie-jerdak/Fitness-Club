using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FitnessClub_Test.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarService _calendarService;

        public CalendarController(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        [HttpGet("events")]
        public async Task<IActionResult> GetEvents(DateTime? start, DateTime? end, int userID)
        {            
            var role = User.Claims.First(c => c.Type == ClaimTypes.Role).Value;

            var events = await _calendarService.GetCalendarEvents(role, userID, start, end);
            return Ok(events);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateCalendarEventDTO dto)
        {
            if (dto.Start >= dto.End)
                return BadRequest(new { message = "Start time must be before end time." });

            try
            {
                var created = await _calendarService.CreateEvent(dto);
                if (!created)
                    return BadRequest(new { message = "Event could not be created." });

                return Ok(new { message = "Event created successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update-events/{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateCalendarEventDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.Id)
                return BadRequest("Event ID mismatch.");

            try
            {
                var updated = await _calendarService.UpdateEvent(dto);
                if (!updated)
                    return NotFound("Event not found.");

                return Ok(new { message = "Event updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("events/{id}")]
        public async Task<IActionResult> DeleteEvent(int id, [FromQuery] bool applySeries = false)
        {
            try
            {
                var deleted = await _calendarService.DeleteEvent(id, applySeries);

                if (!deleted)
                    return NotFound(new { message = "Event not found." });

                return Ok(new { message = applySeries ? "Recurring series deleted." : "Event deleted." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
