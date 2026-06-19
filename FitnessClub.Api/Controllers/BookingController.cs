using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.Services;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessClub_Test.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<ClassesController> _logger;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }


        [HttpGet("get-bookings")]
        public async Task<ActionResult<List<BookingViewDTO>>> GetBookings()
        {
            try
            {
                var result = await _bookingService.GetAllBookings();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult>Create(BookingDTO dto)
        {
            try
            {
                await _bookingService.CreateBooking(dto);
                return Ok(new { message = "Booking successful" });
            }
            catch (DbUpdateException dbEx)
            {
                return BadRequest(new { message = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{bookingID}")]
        public async Task<ActionResult<BookingViewDTO>> GetById(int bookingID)
        {
            try
            {
                var booking = await _bookingService.GetBookingById(bookingID);

                if (booking == null)
                    return NotFound(new { message = "Booking not found" });

                return Ok(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: /api/booking/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookingDTO dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid data" });

            try
            {
                var updated = await _bookingService.UpdateBooking(id, dto);

                if (!updated)
                    return NotFound(new { message = "Booking not found" });

                return Ok(new { message = "Booking updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var result = await _bookingService.DeleteBookingAsync(id);

            if (!result)
                return NotFound(new { message = "Booking not found" });

            return Ok(new { message = "Booking deleted successfully" });
        }


        [HttpGet("{classId}/clients")]
        public async Task<ActionResult<List<ClientDTO>>> GetClientsByClass(int classId)
        {
            var clients = await _bookingService.GetClientsByClass(classId);

            if (clients == null || !clients.Any())
                return NotFound();

            return Ok(clients);
        }
    }
}