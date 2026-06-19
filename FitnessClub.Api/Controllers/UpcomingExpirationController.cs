using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.Services;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitnessClub_Test.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UpcomingExpirationController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly INotificationService _notifyService;


        public UpcomingExpirationController(ISubscriptionService subscriptionService, INotificationService notifyService)
        {
            _subscriptionService = subscriptionService;
            _notifyService = notifyService;
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcoming()
        {
            var result = await _subscriptionService.GetUpcomingExpirations();
            return Ok(result);
        }

        [HttpPost("send/{userId}")]
        public async Task<IActionResult> SendToUser(int userId)
        {
            await _notifyService.GenerateAndSendNotificationForUser(userId);
            return NoContent();
        }
        
        [HttpPost("renew/{membershipId}")]
        public async Task<IActionResult> RenewSubscription(int membershipId)
        {
            try
            {

                InvoiceDTO invoiceDto = await _subscriptionService.RenewSubscriptionService(membershipId);
                return Ok(invoiceDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "An unexpected error occurred while renewing the subscription."
                });
            }
        }


    }
}
