using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessClub_Test.Api.Controllers
{
    [Authorize(Roles = "Admin, Coach")]
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;
        public ExerciseController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExercises()
        {
            var exercises = await _exerciseService.GetAllExercisesAsync();

            if (exercises == null || !exercises.Any())
                return Ok(new List<ExerciseDTO>());

            return Ok(exercises);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetExercise(int id)
        {
            var exercise = await _exerciseService.GetExerciseByIdAsync(id);
            return exercise == null ? NotFound() : Ok(exercise);
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateExercise([FromBody] ExerciseDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _exerciseService.CreateExerciseAsync(dto);
            return Ok(id);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateExercise([FromBody] ExerciseDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    message = "Invalid data",
                    errors = ModelState
                });

            try
            {
                var updated = await _exerciseService.UpdateExerciseAsync(dto);

                if (!updated)
                {
                    return NotFound(new
                    {
                        message = "Exercise not found"
                    });
                }

                return Ok(new
                {
                    message = "Exercise updated successfully"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    message = "Internal server error"
                });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteExercise(int id)
        {
            var deleted = await _exerciseService.DeleteExerciseAsync(id);

            if (!deleted)
                return NotFound(new { message = "Exercise not found" });

            return Ok(new { message = "Exercise deleted" });
        }

        [HttpPost("bulk-delete")]
        public async Task<IActionResult> BulkDelete([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest(new { message = "No exercises selected" });

            await _exerciseService.BulkDeleteAsync(ids);

            return Ok(new { message = "Exercises deleted successfully" });
        }
    }
}
