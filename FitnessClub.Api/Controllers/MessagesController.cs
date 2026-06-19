using Microsoft.AspNetCore.Mvc;
using FitnessClub_Test.Dtos;
using FitnessClub_Test.Core.Interfaces;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace FitnessClub_Test.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MessageDto dto)
        {
            await _messageService.SubmitMessageAsync(dto);
            return Ok(new { message = "Message submitted successfully." });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetMessagesByUser(int userId, int page = 1, int pageSize = 8)
        {
            var pagedMessages = await _messageService.GetMessagesByUserAsync(userId, page, pageSize);
            return Ok(pagedMessages);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("user/{userId}/all")]
        public async Task<IActionResult> GetAllMessagesByUser(int userId)
        {
            var allMessages = await _messageService.GetMessagesByUserAsync(userId, 1, int.MaxValue);
            return Ok(allMessages.Items);
        }

    }
}
