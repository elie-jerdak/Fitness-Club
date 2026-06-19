using FitnessClub_Test.Core.Helpers;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(string recipient, string subject, string fullName, int daysRemaining);
        Task SendEmail(string recipient, string subject, string header, string firstName, string messageBody);
        Task SendOnlineInvoiceEmail(string recipient, string subject, InvoiceDetails invoice);
    }
}