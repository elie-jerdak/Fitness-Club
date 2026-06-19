using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessClub_Test.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly IClassService _classService;
        private readonly ILogger<ClassesController> _logger;

        public ClassesController(IClassService classService, ILogger<ClassesController> logger)
        {
            _classService = classService;
            _logger = logger;
        }

        
        [HttpGet("get-classes")]
        public async Task<ActionResult<List<ClassDTO>>> GetClasses()
        {
            try
            {
                var result = await _classService.GetAllClasses();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving classes.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("{ClassID}")]
        public async Task<ActionResult<ClassDTO>> GetClassById(int ClassID)
        {
            try
            {
                var result = await _classService.GetClassById(ClassID);
                if (result == null)
                    return NotFound($"Class with ID {ClassID} not found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving class with ID {ClassID}.");
                return StatusCode(500, "Internal server error.");
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost("create-class")]
        public async Task<ActionResult<ClassDTO>> CreateClass(ClassDTO newClass)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .ToList();

                return BadRequest(errors);
            }
                
            try
            {
                var created = await _classService.CreateClass(newClass);
                return CreatedAtAction(nameof(GetClassById), new { id = created.ClassID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new class.");
                return StatusCode(500, "Internal server error.");
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClass(int id, ClassDTO updatedClass)
        {
            if (id != updatedClass.ClassID)
                return BadRequest("ID mismatch.");

            try
            {
                var success = await _classService.UpdateClass(updatedClass);

                if (!success)
                    return NotFound($"Class with ID {id} not found.");

                return Ok("Class updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating class with ID {id}.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteClass(int id)
        {
            try
            {
                var success = await _classService.DeleteClass(id);
                if (!success)
                    return NotFound($"Class with ID {id} not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting class with ID {id}.");
                return StatusCode(500, "Internal server error.");
            }
        }

        
    }
}
