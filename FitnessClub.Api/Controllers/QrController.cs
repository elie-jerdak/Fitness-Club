using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FitnessClub_Test.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QrController : ControllerBase
    {
        private readonly IQRCodeService _qrService;
        private readonly IAttendanceService _attendanceService;

        public QrController(IQRCodeService qrService, IAttendanceService attendanceService)
        {
            _qrService = qrService;
            _attendanceService = attendanceService;
        }

        // used for check in out
        [Authorize]
        [HttpPost("generate")]
        public async Task<IActionResult> Generate()
        {
            
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            var type = await _attendanceService.ResolveNextQrType(userId);
            if (type != "IN" && type != "OUT")
                return BadRequest("Invalid QR type");


            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var device = Request.Headers["User-Agent"].ToString();


            var token = await _qrService.GenerateQRCode(userId, type, ip, device);

            return Ok(new { token,  type});
        }

        // used for create first time user
        [Authorize]
        [HttpPost("generate/fixed-temp")]
        public IActionResult GenerateFixedTemp()
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var device = Request.Headers["User-Agent"].ToString();

            var token =  _qrService.GenerateTempFixedQRCode(ip, device);

            return Ok(new
            {
                token
            });
        }

        //used for updating
        [Authorize]
        [HttpPost("generate/updated/{UserID}")]
        public async Task<IActionResult> GenerateUpdated(int UserID)
        {
            var (success, result) = await _qrService.UpdateUserQrAsync(UserID);

            if (!success)
                return NotFound(result);

            return Ok(new
            {
                success,
                qrValue = result
            });
        }

    }
}
