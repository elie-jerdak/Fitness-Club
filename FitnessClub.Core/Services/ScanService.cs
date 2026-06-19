using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Services
{
    public class ScanService : IScanService
    {
        private readonly FitnessClubDbContext _context;
        public ScanService(FitnessClubDbContext context)
        {
            _context = context;
        }
        public async Task<(bool Success, string Message)> ScanAsync(string token, string ip, string device)
        {
            var qr = await _context.QrTokens
                .FirstOrDefaultAsync(x => x.Token == token);

            if (qr == null)
                return (false, "Invalid QR");

            if (qr.Used)
                return (false, "QR already used");

            if (qr.ExpiresAt < DateTime.UtcNow)
                return (false, "QR expired");

            if (qr.Type == "IN")
                return await HandleCheckIn(qr, ip, device);

            if (qr.Type == "OUT")
                return await HandleCheckOut(qr, ip, device);

            return (false, "Invalid QR type");
        }

        private async Task<(bool, string)> HandleCheckIn(QrToken qr, string ip, string device)
        {
            var openSession = await _context.CheckingInOuts
                .FirstOrDefaultAsync(x =>
                    x.UserId == qr.UserId &&
                    x.TimeOut == null);

            if (openSession != null)
                return (false, "User already checked in");

            _context.CheckingInOuts.Add(new CheckingInOut
            {
                UserId = qr.UserId,
                TimeIn = DateTime.UtcNow,
                QrToken = qr.Token,
                Result = "CheckIn Granted",
                ScanIp = ip,
                Timestamp = DateTime.UtcNow,
                ScanDevice = device
            });

            qr.Used = true;
            qr.UsedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return (true, "Check-in successful");
        }

        private async Task<(bool, string)> HandleCheckOut(QrToken qr, string ip, string device)
        {
            var openSession = await _context.CheckingInOuts
                .FirstOrDefaultAsync(x =>
                    x.UserId == qr.UserId &&
                    x.TimeOut == null);

            if (openSession == null)
                return (false, "No active session");

            openSession.TimeOut = DateTime.UtcNow;
            openSession.QrToken = qr.Token;
            openSession.ScanIp = ip;
            openSession.ScanDevice = device;
            openSession.Timestamp = DateTime.UtcNow;
            openSession.Result = "CheckOut Complete";

            qr.Used = true;
            qr.UsedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return (true, "Check-out successful");
        }

    }
}
