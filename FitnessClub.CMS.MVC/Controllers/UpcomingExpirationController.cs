using FitnessClub_Test.CMS.MVC.Services;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FitnessClub_Test.CMS.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UpcomingExpirationController : BaseController
    {
        private readonly ApiAuthClient _api;
        private readonly FitnessClubDbContext _context;
        private readonly ILogger<UpcomingExpirationController> _logger;

        public UpcomingExpirationController(ApiAuthClient api, ILogger<UpcomingExpirationController> logger,
            FitnessClubDbContext context) : base(api)
        {
            _api = api;
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> UpcomingExpirations()
        {
            try
            {
                var client = await _api.GetAuthorizedClientAsync();
                var response = await client.GetAsync("upcoming-expiration/upcoming");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API call failed with status code: {StatusCode}", response.StatusCode);
                    return View(new List<ExpiringSubscriptionDto>()); // Return empty list to avoid null model
                }

                var data = await response.Content.ReadFromJsonAsync<List<ExpiringSubscriptionDto>>();

                // Ensure non-null model
                return View("UpcomingExpirationView",data ?? new List<ExpiringSubscriptionDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching upcoming expirations.");
                return View(new List<ExpiringSubscriptionDto>());
            }
        }

        public async Task<IActionResult> SendNotification(int userId)
        {
            try
            {
                var client = await _api.GetAuthorizedClientAsync();
                var response = await client.PostAsync($"upcoming-expiration/send/{userId}", null);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API call failed with status code: {StatusCode}", response.StatusCode);
                    TempData["Error"] = "Failed to send notification.";
                    return RedirectToAction("UpcomingExpirations");
                }
                //add toast code here
                TempData["Success"] = "Notification sent successfully!";
                // Return to model
                return RedirectToAction("UpcomingExpirations");
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Error sending notification to user {userId}");
                TempData["Error"] = "An error occurred while sending the notification.";
                return StatusCode(500, "Server error.");
            }

        }

        [HttpPost]
        public async Task<IActionResult> Renew(int membershipId)
        {
            try
            {
                var client = await _api.GetAuthorizedClientAsync();
                var response = await client.PostAsync($"upcoming-expiration/renew/{membershipId}", null);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var invoice = JsonConvert.DeserializeObject<InvoiceDTO>(json);

                    return View("InvoiceView", invoice);
                }

                return RedirectToAction("UpcomingExpirations");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renewing membership");
                TempData["Error"] = "Renewal failed.";
                return RedirectToAction("UpcomingExpirations");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RenewOnly(int membershipId)
        {
            try
            {
                var client = await _api.GetAuthorizedClientAsync();
                var response = await client.PostAsync($"upcoming-expiration/renew/{membershipId}", null);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Renewal Completed.";
                    return RedirectToAction("UpcomingExpirations");
                }

                TempData["Error"] = "Renewal failed.";
                return RedirectToAction("UpcomingExpirations");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renewing membership");
                TempData["Error"] = "Renewal failed.";
                return RedirectToAction("UpcomingExpirations");
            }
        }

        // The following methods are for online payments

        [HttpPost]
        public async Task<IActionResult> PayOnline(int membershipId)
        {
            var client = await _api.GetAuthorizedClientAsync();

            var dto = new CreateStripeSessionDTO
            {
                MembershipID = membershipId
            };

            Console.WriteLine($"Api Call: {client.BaseAddress}payments/create-session");

            // Call API to create Stripe session
            var response = await client.PostAsJsonAsync("payments/create-session", dto);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to initiate online payment.";
                return RedirectToAction("UpcomingExpirations");
            }

            // Read the URL returned by the API
            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            if (result == null || !result.ContainsKey("url"))
            {
                TempData["Error"] = "Invalid response from payment API.";
                return RedirectToAction("UpcomingExpirations");
            }

            var stripeUrl = result["url"];

            // Redirect user to Stripe checkout page
            return Redirect(stripeUrl);
        }

        public async Task<IActionResult> Success([FromQuery] string session_id)
        {
            if (string.IsNullOrEmpty(session_id))
            {
                TempData["Error"] = "Invalid payment session.";
                return RedirectToAction("UpcomingExpirations");
            }

            var payment = await _context.SubscriptionPayments
                .FirstOrDefaultAsync(p => p.StripeSessionId == session_id);

            if (payment != null)
            {
                TempData["Success"] = "Payment completed successfully!";
            }
            else
            {
                TempData["Info"] = "Payment received. Confirmation in progress...";
            }

            return RedirectToAction("UpcomingExpirations");
        }

        public IActionResult Cancel()
        {
            TempData["Error"] = "Payment was cancelled.";
            return RedirectToAction("UpcomingExpirations");
        }
    }
}
    