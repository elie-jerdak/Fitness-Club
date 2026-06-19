using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Dtos
{
    public class BookingCreateDTO
    {
        // Required IDs (bound from dropdowns)
        public int BookingID { get; set; }
        public int ClassID { get; set; }
        public int ClientID { get; set; }

        // Booking details
        public string Type { get; set; }
        public string Status { get; set; }

        // Dropdown data
        public List<ClassDTO> Classes { get; set; } = new();
        public List<EnrolledClientDTO> Clients { get; set; } = new();
    }
}
