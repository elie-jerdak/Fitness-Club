using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessClub_Test.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PremadeProgramsController : ControllerBase
    {
        private readonly IPremadeProgramsService _premadeProgramsService;

        public PremadeProgramsController(IPremadeProgramsService premadeProgramsService)
        {
            _premadeProgramsService = premadeProgramsService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<ActionResult<List<PremadeProgramsDTO>>> GetAllPrograms()
        {
            var data = await _premadeProgramsService.GetPremadeProgramsAsync();
            return Ok(data);
        }

        [Authorize(Roles = "Admin, Coach")]
        [HttpPut("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _premadeProgramsService.DeleteProgram(id);
            if (!success)
                return NotFound();
            return NoContent();
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<PremadeProgramsDTO>> GetPremadeProgramById(int id)
        {
            var program = await _premadeProgramsService.GetPremadeProgramByIdAsync(id);

            if (program == null)
                return NotFound();

            return Ok(program);
        }

        [Authorize(Roles = "Admin, Coach")]
        [HttpPost("save")]
        public async Task<IActionResult> UpdatePremadeProgram(PremadeProgramsDTO dto)
        {
            Console.WriteLine($"dto inside the api {dto.CoverImage} ");
            var success = await _premadeProgramsService.UpdatePremadeProgramAsync(dto);

            if (!success)
                return BadRequest("Update failed");

            return Ok();
        }

        [Authorize(Roles = "Admin, Coach")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateProgram([FromBody] PremadeProgramsDTO dto)
        {
            Console.WriteLine("Inside API");
            Console.WriteLine("CoverImage received: " + dto.CoverImage);

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    errors = ModelState.ToDictionary(
                        kvp => kvp.Key.Split('.').Last(),
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    )
                });
            }

            bool isCreated = await _premadeProgramsService.CreatePremadeProgramAsync(dto);

            if (!isCreated)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while saving the program. Please try again later."
                });
            }

            return Ok(new { message = "Program saved successfully" });
        }
    }
}
