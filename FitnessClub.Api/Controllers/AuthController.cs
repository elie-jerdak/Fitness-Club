using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FitnessClub.Dtos;
using FitnessClub.Core.Interfaces;
using FitnessClub.Core.NewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Numerics;

namespace FitnessClub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly JwtTokenGen _TokenGen;

        public AuthController(IAuthService authService, IConfiguration config, IUserService userService)
        {
            _authService = authService;
            _TokenGen = new JwtTokenGen(config);
            _userService = userService;
        }

        [HttpPost("register")]

        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(new { message = result.Message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _authService.LoginAsync(dto);
            if (user == null)
                return Unauthorized("Invalid email or password.");
            var token = _TokenGen.GenerateToken(user);
            return Ok(new
            {
                Token = token,
                User = new
                {
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Role,
                    user.QrCode,
                    user.PhoneNumber
                }
            });
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDto dto)
        {
            var result = await _authService.DeleteAccountAsync(dto);
            if (!result)
                return NotFound(new { message = "User not found or already deleted." });

            return Ok(new { message = "Account deleted successfully." });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var email = User.Identity.Name;

            var user = await _authService.GetUserByUsernameAsync(email);

            if (user == null) return NotFound();

            var profileDto = new UserProfileDto
            {
                FirstName = user.First_Name,
                SecondName = user.Last_Name,
                Email = user.Email,
                Role = user.Role,
                QrCode = user.QrCode,
                PhoneNumber = user.Phone_Number
            };

            if (user.Role == "Client")
            {
                profileDto.Weight = user.Weight;
                profileDto.Height = user.Height;
                profileDto.MedicalIssues = user.MedicalIssues;
                profileDto.Target = user.Target;
            }
            else if (user.Role == "Coach")
            {
                profileDto.Salary = user.Salary;
                profileDto.Experience = user.Experience;
                profileDto.Specialty = user.Specialty;
            }

            return Ok(profileDto);
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var email = User.Identity.Name;

            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            try
            {
                var updatedUser = await _userService.UpdateUserByEmailAsync(email, updateDto);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}