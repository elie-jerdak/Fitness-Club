using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Dtos
{
    public class InvoiceDTO
    {
        public int InvoiceID { get; set; }
        public UserDTO User { get; set; }
        public MemberDTO member { get; set; }
        public DateTime Date { get; set; }
        public string PaymentMethod { get; set; }
        public string Currency { get; set; }
        
    }
}
