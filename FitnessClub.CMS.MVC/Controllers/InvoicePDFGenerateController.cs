using FitnessClub_Test.CMS.MVC.Services;
using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.X509;
using System.Threading.Tasks;

namespace FitnessClub_Test.CMS.MVC.Controllers
{
    public class InvoicePDFGenerateController : BaseController
    {
        private readonly ApiAuthClient _apiAuthClient;
        private readonly ISubscriptionService _invoiceService;
        private readonly IPDFGeneratingService _pdfService;

        public InvoicePDFGenerateController(ISubscriptionService invoiceService, IPDFGeneratingService pdfService, ApiAuthClient api) : base (api)
        {
            _invoiceService = invoiceService;
            _apiAuthClient = api;
            _pdfService = pdfService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var client = _apiAuthClient.GetAuthorizedClientAsync();
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
                return NotFound();

            var pdfBytes = _pdfService.GenerateInvoicePdf(invoice);
            return File(pdfBytes, "application/pdf", $"Invoice_{invoice.InvoiceID}.pdf");
        }
    }

}
