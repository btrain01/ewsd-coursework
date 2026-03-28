using backend_app.Attributes;
using backend_app.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ServiceFilter(typeof(AuthenticationAttribute))]
    public class UserController(UserService userService) : ControllerBase
    {
        [HttpGet("dashboard/{userId}")]
        public async Task<ActionResult> GetDashboard(int userId)
        {
            var dashboard = await userService.GetUserDashboard(userId);
            if (dashboard == null)
                return NotFound("User not found");
            return Ok(dashboard);
        }

        [HttpGet("assignment/student/{userId}")]
        public async Task<ActionResult> GetTutorByStudentId(int userId)
        {
            var tutor = await userService.GetTutorByStudentId(userId);
            if (tutor == null)
                return NotFound("No tutor assigned");
            return Ok(tutor);
        }

        [HttpGet("assignment/tutor/{userId}")]
        public async Task<ActionResult> GetStudentsByTutorId(int userId)
        {
            var tutor = await userService.GetStudentsByTutorId(userId);
            if (tutor == null)
                return NotFound("No Students assigned");
            return Ok(tutor);
        }

        [HttpGet("meetings/{userId}")]
        public async Task<ActionResult> GetMeetings(int userId)
        {
            var meetings = await userService.GetUserMeetings(userId);
            if (meetings == null)
                return NotFound("No meetings found");
            return Ok(meetings);
        }

        [HttpGet("documents/{userId}")]
        public async Task<ActionResult> GetDocuments(int userId)
        {
            var documents = await userService.GetUserDocuments(userId);
            if (documents == null)
                return NotFound("No documents found");
            return Ok(documents);
        }

        [HttpGet("messages/{userId}")]
        public async Task<ActionResult> GetMessages(int userId)
        {
            var messages = await userService.GetUserMessages(userId);
            if (messages == null)
                return NotFound("No messages found");
            return Ok(messages);
        }
    }
}