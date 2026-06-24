using FitnessClub_Test.Core.Helpers;
using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FitnessClub_Test.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly FitnessClubDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IPaymentService _paymentService;
        private readonly StripeOptions _stripeOptions;
        private readonly IConfiguration _config;

        public PaymentsController(FitnessClubDbContext context, IOptions<StripeOptions> stripeOptions,
            IEmailService emailService, IConfiguration config, IPaymentService paymentService)
        {
            _context = context;
            _stripeOptions = stripeOptions.Value;
            _emailService = emailService;
            _config = config;
            _paymentService = paymentService;
        }

        // CREATE CHECKOUT SESSION
        [HttpPost("create-session")]
        public async Task<IActionResult> CreateSession([FromBody] CreateStripeSessionDTO dto)
        {
            var membership = await _context.Memberships
                .Include(m => m.Client)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == dto.MembershipID);

            if (membership == null)
                return BadRequest("Membership not found");

            var amount = GetAmount(membership.Type);
            var baseUrl = _config["Urls:CmsBaseUrl"];

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(amount * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Subscription Payment"
                        }
                    },
                    Quantity = 1
                }
            },
                SuccessUrl = $"{baseUrl}UpcomingExpiration/Success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{baseUrl}UpcomingExpiration/Cancel",

                Metadata = new Dictionary<string, string>
            {
                { "membershipID", dto.MembershipID.ToString() }
            }
            };

            try
            {
                var SuccessUrl = $"{baseUrl}UpcomingExpiration/Success?session_id={{CHECKOUT_SESSION_ID}}";
                var CancelUrl = $"{baseUrl}UpcomingExpiration/Cancel";

                Console.WriteLine($"//////////////SuccessUel: {SuccessUrl}");
                Console.WriteLine($"/////////////////CancelUrl: {CancelUrl}");

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                return Ok(new
                {
                    url = session.Url
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            try
            {
                var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _stripeOptions.WebhookSecret
                );

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;
                    await _paymentService.HandleCheckoutSessionCompletedAsync(session);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("WEBHOOK ERROR: " + ex);
                // Always return 200 to Stripe to avoid retries for handled errors
                return Ok();
            }

        }

        private decimal GetAmount(string type) => type switch
        {
            "Monthly" => 29.99m,
            "Yearly" => 324.99m,
            "Trial" => 0m,
            _ => throw new Exception("Invalid type")
        };

    }

}
