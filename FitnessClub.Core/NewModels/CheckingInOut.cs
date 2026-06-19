using System;
using System.Collections.Generic;

namespace FitnessClub_Test.Core.NewModels;

public partial class CheckingInOut
{
    public int Id { get; set; }

    public DateTime TimeIn { get; set; }

    public DateTime? TimeOut { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; }

    // --- QR scan audit ---

    public string QrToken { get; set; }       // The token scanned by the user
    public string ScanIp { get; set; }        // IP address of the device scanning
    public string ScanDevice { get; set; }    // Device info (user agent)
    public string Result { get; set; }        // Optional: "Success", "AlreadyCheckedIn", "InvalidToken", etc.
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;  // When the scan attempt happened
}

