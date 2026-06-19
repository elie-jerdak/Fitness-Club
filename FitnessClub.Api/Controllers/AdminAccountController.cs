using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FitnessClub_Test.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAccountController : ControllerBase
    {
        private readonly IAdminAccountService _adminAccountService;
        private readonly ILogger<AdminAccountController> _logger;
        public AdminAccountController(IAdminAccountService adminAccountService, ILogger<AdminAccountController> logger)
        {
            _adminAccountService = adminAccountService;
            _logger = logger;   
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdminAccountDTO>> GetUserById(int id)
        {
            var adminDto = await _adminAccountService.GetAdminByIdAsync(id);

            if (adminDto == null)
            {
                return NotFound(new
                {
                    errors = new[] { "User not found." }
                });
            }

            return Ok(adminDto);
        }

        [HttpPost]
        [Route("editapi")]
        public async Task<IActionResult> Update([FromBody] AdminAccountDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return NotFound();
            }

            int userId = int.Parse(userIdClaim.Value);

            var (Success, Error) = await _adminAccountService.UpdateAsync(userId, dto);

            if (!Success)
                return BadRequest(
                    new Dictionary<string, string[]>
{
                    { "_", new[] { Error } }
                });

            return Ok();
        }
    }
}
