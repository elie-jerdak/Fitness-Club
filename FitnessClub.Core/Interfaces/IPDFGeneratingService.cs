using FitnessClub_Test.Dtos;

namespace FitnessClub_Test.Core.Services
{
    public interface IPDFGeneratingService
    {
        byte[] GenerateInvoicePdf(InvoiceDTO invoice);
    }
}