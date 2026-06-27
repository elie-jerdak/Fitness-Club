using FitnessClub_Test.Core.Helpers;
using FitnessClub_Test.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
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
            // SMTP Working Implementation however blocked by render because i am on the free tier.
            //var email = _config.GetValue<string>("EmailConfiguration:email");
            //var password = _config.GetValue<string>("EmailConfiguration:pwd");
            //var host = _config.GetValue<string>("EmailConfiguration:host");
            //var port = _config.GetValue<int>("EmailConfiguration:port");

            //Console.WriteLine($" email {recipient}  password '{password}' host: {host} port: {port} ");


            //var smtpClient = new SmtpClient(host, port)
            //{
            //    EnableSsl = true,
            //    UseDefaultCredentials = false,

            //    Credentials = new NetworkCredential(email, password)
            //};

            //// Load HTML template
            ////var basePath = _config["Paths:TemplateFolderPath"];
            ////var templatePath = Path.Combine(basePath, "SubscriptionReminderTemplate.html");

            ////var htmlTemplate = await File.ReadAllTextAsync(templatePath);
            //var templatePath = Path.Combine(
            //    Directory.GetCurrentDirectory(),
            //    "Templates",
            //    "SubscriptionReminderTemplate.html"
            //);

            //var htmlTemplate = await File.ReadAllTextAsync(templatePath);

            //Console.WriteLine("/////////////////" + templatePath);
            //Console.WriteLine(File.Exists(templatePath));

            //// Build the messageBody based on daysRemaining
            //string messageBody;
            //if (daysRemaining == 0)
            //    messageBody = "Your subscription expires today.";
            //else if (daysRemaining < 0)
            //{
            //    if(daysRemaining == -1) messageBody = $"Your subscription expired {-daysRemaining} day ago.";
            //    else messageBody = $"Your subscription expired {-daysRemaining} days ago.";
            //}

            //else
            //{
            //    if (daysRemaining == 1) messageBody = $"Your subscription will expire in {daysRemaining} day.";
            //    else messageBody = $"Your subscription will expire in {daysRemaining} days"; 
            //}

            //// Replace placeholders
            //var htmlBody = htmlTemplate
            //    .Replace("{{FullName}}", fullName)
            //    .Replace("MessageBody", messageBody);

            //// Create the MailMessage
            //var message = new MailMessage(email!, recipient, subject, htmlBody)
            //{
            //    IsBodyHtml = true
            //};

            //await smtpClient.SendMailAsync(message);

            // Resend Implementation
            var templatePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Templates",
                "SubscriptionReminderTemplate.html"
            );

            var html = await File.ReadAllTextAsync(templatePath);

            html = html.Replace("{{FullName}}", fullName);
            html = html.Replace("{{DaysRemaining}}", daysRemaining.ToString());

            var apiKey = _config["RESEND_API_KEY"];

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Add(
                "Authorization",
                $"Bearer {apiKey}"
            );
            recipient = "jardakelie@gmail.com";
            var body = new
            {
                from = "onboarding@resend.dev",
                to = recipient,
                subject = subject,
                html = html
            };

            var json = JsonConvert.SerializeObject(body);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync(
                "https://api.resend.com/emails",
                content
            );

            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine(result);

            response.EnsureSuccessStatusCode();
        }

        public async Task SendEmail(string recipient, string subject, string header, string firstName, string messageBody)
        {
            var email = _config.GetValue<string>("EmailConfiguration:email");
            var password = _config.GetValue<string>("EmailConfiguration:pwd");
            var host = _config.GetValue<string>("EmailConfiguration:host");
            var port = _config.GetValue<int>("EmailConfiguration:port");

            var smtpClient = new SmtpClient(host, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(email, password)
            };

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

        //used for online payment SMTP version
        //public async Task SendOnlineInvoiceEmail(string recipient, string subject, InvoiceDetails invoice)
        //{
        //    var senderEmail = _config.GetValue<string>("EmailConfiguration:email");
        //    var password = _config.GetValue<string>("EmailConfiguration:pwd");
        //    var host = _config.GetValue<string>("EmailConfiguration:host");
        //    var port = _config.GetValue<int>("EmailConfiguration:port");

        //    using var smtpClient = new SmtpClient(host, port)
        //    {
        //        EnableSsl = true,
        //        UseDefaultCredentials = false,
        //        Credentials = new NetworkCredential(senderEmail, password)
        //    };

        //    var basePath = _config["Paths:TemplateFolderPath"];
        //    var templatePath = Path.Combine(basePath, "OnlineReceipt.html");
        //    var htmlTemplate = await File.ReadAllTextAsync(templatePath);

        //    // Replace placeholders
        //    var htmlBody = htmlTemplate
        //        .Replace("{{invoiceNumber}}", invoice.InvoiceNumber)
        //        .Replace("{{customerName}}", invoice.CustomerName)
        //        .Replace("{{customerEmail}}", invoice.CustomerEmail)
        //        .Replace("{{country}}", invoice.Country)
        //        .Replace("{{amount}}", invoice.Amount.ToString("F2"))
        //        .Replace("{{currency}}", invoice.Currency)
        //        .Replace("{{transactionId}}", invoice.TransactionId)
        //        .Replace("{{receiptUrl}}", invoice.ReceiptUrl)
        //        .Replace("{{cardBrand}}", invoice.CardBrand?.ToUpper())
        //        .Replace("{{last4}}", invoice.Last4)
        //        .Replace("{{paidAt:yyyy-MM-dd}}", invoice.PaidAt.ToString("yyyy-MM-dd"));

        //    var message = new MailMessage(senderEmail, recipient, subject, htmlBody)
        //    {
        //        IsBodyHtml = true
        //    };

        //    await smtpClient.SendMailAsync(message);

        //    Console.WriteLine($"Invoice email sent to {recipient} for {invoice.InvoiceNumber}");
        //}

        public async Task SendOnlineInvoiceEmail(string recipient, string subject, InvoiceDetails invoice)
        {
            var basePath = _config["Paths:TemplateFolderPath"];
            var templatePath = Path.Combine(basePath, "OnlineReceipt.html");
            var htmlTemplate = await File.ReadAllTextAsync(templatePath);

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

            var apiKey = _config["RESEND_API_KEY"];

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var body = new
            {
                from = "onboarding@resend.dev",
                to = recipient,
                subject = subject,
                html = htmlBody
            };

            var json = JsonConvert.SerializeObject(body);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.resend.com/emails", content);

            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine(result);

            response.EnsureSuccessStatusCode();
        }
    }
}
