using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Dtos
{
    public class CoachEditDTO
    {
        public int UserID { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public string Phone_Number { get; set; }
        public string Role { get; set; }
        public string Address { get; set; }
        public DateTime? DOB { get; set; }
        public string Gender { get; set; }
        public string Photo { get; set; }
        public  string QrCode { get; set; }


        public string Specialty { get; set; }
        public int? YearsExperience { get; set; }
        public string Bio { get; set; }
        public decimal? Salary { get; set; }

        public string ProfileImageBase64 { get; set; }
        public string ProfileImageFileName { get; set; }
    }
}
