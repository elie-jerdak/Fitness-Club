using FitnessClub_Test.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CheckInOutController : ControllerBase
{
    private readonly ICheckInOutService _service;

    public CheckInOutController(ICheckInOutService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CheckInOrOut([FromBody] int userId)
    {
        var result = await _service.CheckInOrOutAsync(userId);
        return Ok(result);
    }
}
