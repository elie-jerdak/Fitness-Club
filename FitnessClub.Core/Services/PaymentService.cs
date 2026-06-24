using FitnessClub_Test.Core.Helpers;
using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly FitnessClubDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        public PaymentService(FitnessClubDbContext context, IEmailService emailService, IConfiguration config)
        {
            _context = context;
            _emailService = emailService;
            _config = config;
        }
        public async Task HandleCheckoutSessionCompletedAsync(Stripe.Checkout.Session session)
        {
            try
            {
                // --- Validate membership ID ---
                if (!session.Metadata.TryGetValue("membershipID", out var membershipIdValue))
                    throw new KeyNotFoundException("membershipID missing");

                if (!int.TryParse(membershipIdValue, out var membershipId))
                    throw new KeyNotFoundException("membershipID invalid");

                // --- Get membership ---
                var membership = await _context.Memberships.FirstOrDefaultAsync(m => m.Id == membershipId);
                if (membership == null)
                    throw new KeyNotFoundException("Membership not found");

                var clientId = membership.ClientId;
                var today = DateOnly.FromDateTime(DateTime.UtcNow);

                var extensionDays = membership.Type switch
                {
                    "Trial" => 7,
                    "Monthly" => 30,
                    "Yearly" => 365,
                    _ => throw new ArgumentOutOfRangeException(nameof(membership.Type), $"Invalid membership type: {membership.Type}")
                };

                membership.StartDate = today;
                membership.EndDate = today.AddDays(extensionDays);

                // --- Save payment to DB ---
                var payment = new SubscriptionPayment
                {
                    Amount = (session.AmountTotal ?? 0) / 100m,
                    Currency = session.Currency?.ToUpper(),
                    Status = "Succeeded",
                    ClientId = clientId,
                    Type = membership.Type,
                    Date = DateTime.UtcNow,
                    PaymentMethod = "Card",
                    StripeSessionId = session.Id,
                    IsActive = true,
                    IsDeleted = false
                };

                Console.WriteLine("//////////////Service: " + payment.ToString());

                _context.SubscriptionPayments.Add(payment);
                await _context.SaveChangesAsync();

                var invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{membershipId}";
                var customerEmail = session.CustomerDetails?.Email ?? "noreply@example.com";

                var invoice = await GetInvoice(session, customerEmail, invoiceNumber);

                await _emailService.SendOnlineInvoiceEmail(customerEmail, $"Invoice {invoiceNumber}", invoice);
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine("ERROR in HandleCheckoutSessionCompletedAsync:");
                Console.WriteLine(ex.ToString());
                // Do not return HTTP codes; webhook controller handles response
                throw;
            }
        }

        private async Task<InvoiceDetails> GetInvoice(Stripe.Checkout.Session session, 
            string customerEmail, string invoiceNumber)
        {
            // --- Get Stripe charge details ---
            var paymentIntentService = new PaymentIntentService();

            var paymentIntent = await paymentIntentService.GetAsync(
                session.PaymentIntentId,
                new PaymentIntentGetOptions
                {
                    Expand = new List<string> { "latest_charge" }
                });


            var charge = paymentIntent.LatestCharge;

            var customerName = session.CustomerDetails?.Name ?? "Valued Customer";
            var country = session.CustomerDetails?.Address?.Country ?? "";

            var amount = (session.AmountTotal ?? 0) / 100m;
            var currency = session.Currency?.ToUpper() ?? "USD";
            var paidAt = charge.Created.ToUniversalTime();// gives time UTC

            var transactionId = charge.Id;
            var receiptUrl = charge.ReceiptUrl;
            var cardBrand = charge.PaymentMethodDetails?.Card?.Brand?.ToUpper();
            var last4 = charge.PaymentMethodDetails?.Card?.Last4;

            var invoice = new InvoiceDetails
            {
                InvoiceNumber = invoiceNumber,
                CustomerName = customerName,
                CustomerEmail = customerEmail,
                Country = country,
                Amount = amount,
                Currency = currency,
                TransactionId = transactionId,
                ReceiptUrl = receiptUrl,
                CardBrand = cardBrand,
                Last4 = last4,
                PaidAt = paidAt
            };

            return invoice;
        }
    }
    
}
