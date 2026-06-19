using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Helpers
{
    public class ReserveSlotResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int CoachUserId { get; set; }
    }
}
