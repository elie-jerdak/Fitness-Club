using FitnessClub_Test.Core.NewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.NewModels
{
    public class QrToken
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string Token { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool Used { get; set; }
        public DateTime? UsedAt { get; set; }

        public string Type { get; set; }   // IN / OUT
        public string IpAddress { get; set; }
        public string Device { get; set; }
    }

}
