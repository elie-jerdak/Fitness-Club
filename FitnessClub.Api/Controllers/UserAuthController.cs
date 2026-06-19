using FitnessClub_Test.Dtos;
using FitnessClub_Test.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FitnessClub_Test.Api.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly IAutorizationService _authService;

        public UserAuthController(IAutorizationService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    errors = ModelState.SelectMany(x => x.Value.Errors).Select(e => e.ErrorMessage).ToArray()
                });
            }


            var result = await _authService.RegisterAsync(dto);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    errors = new[] { result.Error }
                });
            }


            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(dto);

            if (!result.Success)
                return Unauthorized();

            return Ok(new AuthResponseDTO
            {
                Token = result.Token,
                RefreshToken = result.RefreshToken,
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _authService.RevokeRefreshTokenAsync(userId);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("promote/{id}")]
        public async Task<IActionResult> PromoteToAdminAsync(int id)
        {
            var success = await _authService.PromoteToAdminAsync(id);
            if (!success) return NotFound();
            return Ok();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenDTO dto)
        {
            var result = await _authService.RefreshAsync(dto.RefreshToken);

            if (!result.Success)
                return Unauthorized();

            return Ok(new AuthResponseDTO
            {
                Token = result.Token,
                RefreshToken = result.Refresh
            });
        }

    }
}

