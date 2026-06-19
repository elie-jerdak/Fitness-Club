using Microsoft.EntityFrameworkCore;

namespace FitnessClub_Test.Dtos
{
    [Keyless]
    public class TimeSlotHeatmapDto
    {
        public string TimeSlot { get; set; }
        public int Mon { get; set; }
        public int Tue { get; set; }
        public int Wed { get; set; }
        public int Thu { get; set; }
        public int Fri { get; set; }
        public int Sat { get; set; }
        public int Sun { get; set; }
    }
}
