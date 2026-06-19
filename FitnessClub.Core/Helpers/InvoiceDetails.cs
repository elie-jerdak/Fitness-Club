using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Helpers
{
    public class InvoiceDetails
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string ReceiptUrl { get; set; } = string.Empty;
        public string CardBrand { get; set; } = string.Empty;
        public string Last4 { get; set; } = string.Empty;
        public DateTime PaidAt { get; set; }
    }
}
