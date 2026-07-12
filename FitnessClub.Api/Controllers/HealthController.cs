using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FitnessClub_Test.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            Console.WriteLine("Health Endpoint hit successfully");
            return Ok(new
            {
                status = "running",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
