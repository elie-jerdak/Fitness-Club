using FitnessClub_Test.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Interfaces
{
    public interface ISubscriptionService
    {
        Task<InvoiceDTO> RenewSubscriptionService(int membershipId);
        Task<InvoiceDTO> GetInvoiceByIdAsync(int invoiceID);
        Task<List<ExpiringSubscriptionDto>> GetUpcomingExpirations();
    }
}