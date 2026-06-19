using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessClub_Test.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly FitnessClubDbContext _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly IQRCodeService _qrCodeService;
        private readonly IMemberService _memberService;

        public MembersController(FitnessClubDbContext context, IFileUploadService fileUploadService, 
            IMemberService memberService, IQRCodeService qrCodeService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _memberService = memberService;
            _qrCodeService = qrCodeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<MemberDTO>>> GetMembers()
        {
            try
            {
                var members = await _memberService.GetAllMembersAsync();
                if (members == null) members = new List<MemberDTO>();
                return Ok(members);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }


        [HttpGet]
        [Route("{memberId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMemberForUpdate(int memberId)
        {
            var memberDto = await _memberService.GetMemberForUpdateAsync(memberId);
            if (memberDto == null)
                return NotFound();

            return Ok(memberDto);
        }

        [HttpPost]
        [Route("{memberId}")]
        public async Task<IActionResult> EditMember(int memberId, [FromBody] MemberEditDTO dto)
        {
            try
            {
                if (dto == null || dto.MemberID != memberId)
                {
                    return BadRequest(new
                    {
                        errors = new Dictionary<string, string[]>
                {
                    { "_", new[] { "Invalid request" } }
                }
                    });
                }
                
                // REQUIRED
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        errors = ModelState.ToDictionary(
                            k => k.Key,
                            v => v.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        )
                    });
                }
                
                var (success, errors) =
                    await _memberService.UpdateMemberAsync(memberId, dto);
                
                if (success)
                    return Ok();
                
                // Normalize errors
                if (errors == null || errors.Count == 0)
                {
                    return BadRequest(new
                    {
                        errors = new Dictionary<string, string[]>
                {
                    { "_", new[] { "Unknown error occurred." } }
                }
                    });
                }
                
                // Semantic status codes)
                if (errors.ContainsKey("_"))
                {
                    var msg = errors["_"].First();
                
                    if (msg == "Member not found.")
                        return NotFound(new { errors });
                
                    if (msg == "You do not have permission to edit this member.")
                        return StatusCode(StatusCodes.Status403Forbidden,
                            new { errors });
                }
                
                return BadRequest(new { errors });
            }   catch (Exception ex)
                 {
                     // log ex
                     return StatusCode(500, new
                     {
                         errors = new Dictionary<string, string[]>
                         {
                           { "_", new[] { "Server error while updating member." } }
                         }
                     });
                 }

        }

        [HttpGet]
        [Route("{MemberID}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMemberForDelete(int MemberID)
        {
            var dto = await _memberService.GetMemberForDeleteAsync(MemberID);

            if (dto == null)
                return NotFound();

            return Ok(dto);
        }

        [HttpDelete]
        [Route("{MemberID}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMember(int MemberID)
        {
            if(MemberID <= 0)
            {
                return BadRequest();
            }
            var success = await _memberService.DeleteMemberAsync(MemberID);

            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkDeleteMembers([FromBody] List<int> memberIds)
        {
            if (memberIds == null || !memberIds.Any())
                return BadRequest("No user IDs provided.");

            var result = await _memberService.BulkDeleteMemberAsync(memberIds);

            if (result.FailedCount == 0)
            {
                return Ok(new
                {
                    message = "All memberships were deleted successfully.",
                    deletedCount = result.DeletedCount
                });
            }

            return Ok(new
            {
                message = "Some memberships could not be deleted.",
                deletedCount = result.DeletedCount,
                failedCount = result.FailedCount,
                failedUserIds = result.FailedUserIds
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFullMember([FromBody] FullMemberDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Console.WriteLine("Before going to service");
            Console.WriteLine($"Photo inside api: {dto.user.Photo}");

            var result = await _memberService.CreateMemberAsync(dto);

            if (!result.Success)
            {
                if (result.Errors != null)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error);
                }

                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetMemberDetails(int userId)
        {
            // Get member info
            var member = await _memberService.GetMemberByUserIdAsync(userId);
            if (member == null)
                return NotFound("Member not found");

            // Get purchased programs 
            var programs = (await _memberService.GetPurchasedProgramsAsync(userId));

            // Get booked availabilities 
            var sessions = (await _memberService.GetAvailabilitiesBooked(userId));

            // Construct view model
            var viewModel = new MemberDetailViewDTO
            {
                Member = member,
                PurchasedPrograms = programs,
                AvailabilitiesBooked = sessions
            };

            return Ok(viewModel);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetPurchasedPrograms(int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid userId.");

            var programs = await _memberService.GetPurchasedProgramsAsync(userId);

            return Ok(programs);
        }

        [HttpGet("{UserID}")]
        public async Task<IActionResult> GetQrByID(int UserID)
        {
            var QRtoken = await _qrCodeService.GetQrTokenByID(UserID);

            if (QRtoken == null)
                return NotFound();

            return Ok(new
            {
                qrToken = QRtoken
            });
        }


    }
}
