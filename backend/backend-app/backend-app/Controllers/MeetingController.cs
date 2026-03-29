using backend_app.Attributes;
using backend_app.DTOs;
using backend_app.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ServiceFilter(typeof(AuthenticationAttribute))]
    public class MeetingController(MeetingService meetingService) : ControllerBase
    {
        [HttpPost("create/{organiserId}")]
        public async Task<ActionResult> CreateMeeting(int organiserId, [FromBody] CreateMeetingDTO dto)
        {
            var meeting = await meetingService.CreateMeeting(organiserId, dto);
            if (meeting == null)
                return BadRequest("Meeting could not be created");
            return Ok(meeting);
        }
    }
}