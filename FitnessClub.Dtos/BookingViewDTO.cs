using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Dtos
{
    public class BookingViewDTO
    {
        public int BookingID { get; set; }
        public int ClassID { get; set; }
        public int ClientID { get; set; }
        public string ClassName { get; set; }
        public string ClientName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
}
