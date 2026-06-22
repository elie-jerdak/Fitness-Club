using FitnessClub_Test.Core.Helpers;
using FitnessClub_Test.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration configuration)
        {
            _config = configuration;
        }

        // below are two overloaded methods
        public async Task SendEmail(string recipient, string subject, string fullName, int daysRemaining)
        {
            var email = _config.GetValue<string>("EmailConfiguration:email");
            var password = _config.GetValue<string>("EmailConfiguration:pwd");
            var host = _config.GetValue<string>("EmailConfiguration:host");
            var port = _config.GetValue<int>("EmailConfiguration:port");

            var smtpClient = new SmtpClient(host, port);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;

            smtpClient.Credentials = new NetworkCredential(email, password);

            // Load HTML template
            var basePath = _config["Paths:TemplateFolderPath"];
            var templatePath = Path.Combine(basePath, "SubscriptionReminderTemplate.html");

            var htmlTemplate = await File.ReadAllTextAsync(templatePath);

            // Build the messageBody based on daysRemaining
            string messageBody;
            if (daysRemaining == 0)
                messageBody = "Your subscription expires today.";
            else if (daysRemaining < 0)
            {
                if(daysRemaining == -1) messageBody = $"Your subscription expired {-daysRemaining} day ago.";
                else messageBody = $"Your subscription expired {-daysRemaining} days ago.";
            }

            else
            {
                if (daysRemaining == 1) messageBody = $"Your subscription will expire in {daysRemaining} day.";
                else messageBody = $"Your subscription will expire in {daysRemaining} days"; 
            }

            // Replace placeholders
            var htmlBody = htmlTemplate
                .Replace("{{FullName}}", fullName)
                .Replace("MessageBody", messageBody);

            // Create the MailMessage
            var message = new MailMessage(email!, recipient, subject, htmlBody)
            {
                IsBodyHtml = true
            };

            await smtpClient.SendMailAsync(message);
        }

        public async Task SendEmail(string recipient, string subject, string header, string firstName, string messageBody)
        {
            var email = _config.GetValue<string>("EmailConfiguration:email");
            var password = _config.GetValue<string>("EmailConfiguration:pwd");
            var host = _config.GetValue<string>("EmailConfiguration:host");
            var port = _config.GetValue<int>("EmailConfiguration:port");

            var smtpClient = new SmtpClient(host, port);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(email, password);

            // Load HTML template
            var basePath = _config["Paths:TemplateFolderPath"];
            var templatePath = Path.Combine(basePath, "GenericTemplate.html"); // Use a simpler generic template

            var htmlTemplate = await File.ReadAllTextAsync(templatePath);

            // Replace placeholder for message body only
            var htmlBody = htmlTemplate
                .Replace("{{MessageBody}}", messageBody)
                .Replace("{{EmailHeader}}",header)
                .Replace("{{FirstName}}", firstName);

            var message = new MailMessage(email!, recipient, subject, htmlBody)
            {
                IsBodyHtml = true
            };

            await smtpClient.SendMailAsync(message);
        }

        //used for online payment
        public async Task SendOnlineInvoiceEmail(string recipient, string subject, InvoiceDetails invoice)
        {
            var senderEmail = _config.GetValue<string>("EmailConfiguration:email");
            var password = _config.GetValue<string>("EmailConfiguration:pwd");
            var host = _config.GetValue<string>("EmailConfiguration:host");
            var port = _config.GetValue<int>("EmailConfiguration:port");

            using var smtpClient = new SmtpClient(host, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail, password)
            };

            var basePath = _config["Paths:TemplateFolderPath"];
            var templatePath = Path.Combine(basePath, "OnlineReceipt.html");
            var htmlTemplate = await File.ReadAllTextAsync(templatePath);

            // Replace placeholders
            var htmlBody = htmlTemplate
                .Replace("{{invoiceNumber}}", invoice.InvoiceNumber)
                .Replace("{{customerName}}", invoice.CustomerName)
                .Replace("{{customerEmail}}", invoice.CustomerEmail)
                .Replace("{{country}}", invoice.Country)
                .Replace("{{amount}}", invoice.Amount.ToString("F2"))
                .Replace("{{currency}}", invoice.Currency)
                .Replace("{{transactionId}}", invoice.TransactionId)
                .Replace("{{receiptUrl}}", invoice.ReceiptUrl)
                .Replace("{{cardBrand}}", invoice.CardBrand?.ToUpper())
                .Replace("{{last4}}", invoice.Last4)
                .Replace("{{paidAt:yyyy-MM-dd}}", invoice.PaidAt.ToString("yyyy-MM-dd"));

            var message = new MailMessage(senderEmail, recipient, subject, htmlBody)
            {
                IsBodyHtml = true
            };

            await smtpClient.SendMailAsync(message);

            Console.WriteLine($"Invoice email sent to {recipient} for {invoice.InvoiceNumber}");
        }

    }
}
