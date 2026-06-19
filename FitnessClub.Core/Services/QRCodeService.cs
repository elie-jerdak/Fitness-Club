using FitnessClub_Test.Core.NewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class QRCodeService : IQRCodeService
{
    private readonly FitnessClubDbContext _context;

    public QRCodeService(FitnessClubDbContext context)
    {
        _context = context;
    }

    // used later by the website for checkig in out
    public async Task<string> GenerateQRCode(int userId, string type, string ip, string device)
    {
        var qr = new QrToken
        {
            UserId = userId,
            Token = Guid.NewGuid().ToString("N"),
            ExpiresAt = DateTime.UtcNow.AddSeconds(45),
            Used = false,
            Type = type,
            IpAddress = ip,
            Device = device
        };

        _context.QrTokens.Add(qr);
        await _context.SaveChangesAsync();

        return qr.Token;
    }

    // get qr from databse and display it
    public async Task<string> GetQrTokenByID(int UserID)
    {
        return await _context.Users
            .Where(u => u.Id == UserID)
            .Select(u => u.QrCode)
            .FirstOrDefaultAsync();
    }

    // when creating a new user
    public string GenerateTempFixedQRCode(string ip, string device)
    {
        // Generate once
        string QrCode = Guid.NewGuid().ToString("N");

        return QrCode;
    }


    //when updating an existing user
    public async Task<(bool Success, string Result)> UpdateUserQrAsync(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return (false, "User not found");

        var newQrValue = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        return (true, newQrValue);
    }

}
