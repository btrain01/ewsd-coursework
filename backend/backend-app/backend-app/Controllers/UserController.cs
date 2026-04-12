using backend_app.Attributes;
using backend_app.DTOs;
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

        [HttpPost("allocation")]
        public async Task<ActionResult> AllocateTutor([FromBody] AllocationDTO allocationDTO)
        {
            var tutor = await userService.AllocateTutor(allocationDTO);
            if (tutor == null)
                return NotFound("Tutor or Student not found");
            return Ok(tutor);
        }

        [HttpPost("allocation/bulk")]
        public async Task<ActionResult> AllocateTutor([FromBody] BulkAllocationDTO bulkAllocationDTO)
        {
            var tutor = await userService.AllocateStudentsToTutor(bulkAllocationDTO);
            
            if (tutor == null)
                return NotFound("Tutor or Student not found");
            
            return Ok(tutor);
        }

        [HttpGet("reports/students/no-tutor")]
        public async Task<ActionResult> GetUnAllocatedStudents()
        {
            var tutor = await userService.GetUnAllocatedStudents();
            
            if (tutor == null)
                return NoContent();
            
            return Ok(tutor);
        }

        [HttpGet("reports/students/in-active")]
        public async Task<ActionResult> GetInActiveStudents([FromQuery(Name = "days")] int days)
        {
            var tutor = await userService.GetInActiveUsers(days);
            
            if (tutor == null)
                return NoContent();
            
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

        [HttpGet("role/{roleId}")]
        public async Task<ActionResult> GetUsersByRole(int roleId)
        {
            var messages = await userService.GetUsersByRole(roleId);
            
            if (messages == null)
                return NoContent();

            return Ok(messages);
        }

        [HttpGet("role/name/{roleName}")]
        public async Task<ActionResult> GetUsersByRole(string roleName)
        {
            var messages = await userService.GetUsersByRole(roleName);
            
            if (messages == null)
                return NoContent();

            return Ok(messages);
        }
    }
}