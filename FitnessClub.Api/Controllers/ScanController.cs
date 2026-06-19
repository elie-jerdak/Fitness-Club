using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FitnessClub_Test.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScanController : ControllerBase
    {
        private readonly FitnessClubDbContext _context;
        private readonly IScanService _scanService;

        public ScanController(FitnessClubDbContext context, IScanService scanService)
        {
            _context = context;
            _scanService = scanService;
        }

        //    [AllowAnonymous]
        //    [EnableRateLimiting("qr")]
        //    [HttpPost("scan")]
        //    public async Task<IActionResult> Scan(ScanQrDTO dto)
        //    {
        //        // 1. Find the token
        //        var qr = await _context.QrTokens.FirstOrDefaultAsync(x => x.Token == dto.Token);

        //        // 2. Invalid or expired
        //        if (qr == null || qr.Used || qr.ExpiresAt < DateTime.UtcNow)
        //        {
        //            _context.CheckingInOuts.Add(new CheckingInOut
        //            {
        //                UserId = qr?.UserId ?? 0,
        //                TimeIn = DateTime.UtcNow,
        //                QrToken = dto.Token,
        //                ScanIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
        //                ScanDevice = Request.Headers["User-Agent"].ToString(),
        //                Result = "Invalid/Expired",
        //                Timestamp = DateTime.UtcNow
        //            });

        //            await _context.SaveChangesAsync();
        //            return Unauthorized("Invalid/Expired QR Token");
        //        }

        //        // 3. Check if there is an open session
        //        var open = await _context.CheckingInOuts
        //            .FirstOrDefaultAsync(x => x.UserId == qr.UserId && x.TimeOut == null);

        //        CheckingInOut entry;

        //        if (open == null)
        //        {
        //            // Check-in
        //            entry = new CheckingInOut
        //            {
        //                UserId = qr.UserId,
        //                TimeIn = DateTime.UtcNow,
        //                QrToken = qr.Token,
        //                ScanIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
        //                ScanDevice = Request.Headers["User-Agent"].ToString(),
        //                Result = "Granted",
        //                Timestamp = DateTime.UtcNow
        //            };
        //            _context.CheckingInOuts.Add(entry);
        //        }
        //        else
        //        {
        //            // Check-out
        //            open.TimeOut = DateTime.UtcNow;
        //            open.QrToken = qr.Token;
        //            open.ScanIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        //            open.ScanDevice = Request.Headers["User-Agent"].ToString();
        //            open.Result = "Granted";
        //            open.Timestamp = DateTime.UtcNow;
        //        }

        //        // 4. Mark token as used
        //        qr.Used = true;
        //        qr.UsedAt = DateTime.UtcNow;

        //        await _context.SaveChangesAsync();

        //        return Ok("Access Granted");
        //    }
        [AllowAnonymous]
        [HttpPost("scan")]
        public async Task<IActionResult> Scan([FromBody] ScanQrDTO dto)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var device = Request.Headers["User-Agent"].ToString();

            var result = await _scanService.ScanAsync(dto.Token, ip, device);

            if (!result.Success)
                return Unauthorized(result.Message);

            return Ok(result.Message);
        }

    }

}