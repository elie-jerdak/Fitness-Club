using System;
using System.Threading.Tasks;

public interface IQRCodeService
{
    Task<string> GenerateQRCode(int userId, string type, string ip, string device);
    Task<string> GetQrTokenByID(int UserID);
    string GenerateTempFixedQRCode(string ip, string device);
    Task<(bool Success, string Result)> UpdateUserQrAsync(int userId);
}
