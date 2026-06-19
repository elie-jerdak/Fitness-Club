using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Dtos
{
    public class FeedbackDTO
    {
        public int FeedbackId { get; set; }
        public string ClientName { get; set; }
        public string ClientProfilePicture { get; set; }
        public double Stars { get; set; }
        public string Comment { get; set; }
        public string Date { get; set; }
    }
}
