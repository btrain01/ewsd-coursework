using backend_app.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController(StudentService studentService) : ControllerBase
    {
        [HttpGet("dashboard/{studentId}")]
        public async Task<ActionResult> GetDashboard(int studentId)
        {
            var dashboard = await studentService.GetStudentDashboard(studentId);
            if (dashboard == null)
                return NotFound("Student not found");
            return Ok(dashboard);
        }

        [HttpGet("tutor/{studentId}")]
        public async Task<ActionResult> GetTutor(int studentId)
        {
            var tutor = await studentService.GetStudentTutor(studentId);
            if (tutor == null)
                return NotFound("No tutor assigned");
            return Ok(tutor);
        }

        [HttpGet("meetings/{studentId}")]
        public async Task<ActionResult> GetMeetings(int studentId)
        {
            var meetings = await studentService.GetStudentMeetings(studentId);
            if (meetings == null)
                return NotFound("No meetings found");
            return Ok(meetings);
        }

        [HttpGet("documents/{studentId}")]
        public async Task<ActionResult> GetDocuments(int studentId)
        {
            var documents = await studentService.GetStudentDocuments(studentId);
            if (documents == null)
                return NotFound("No documents found");
            return Ok(documents);
        }

        [HttpGet("messages/{studentId}")]
        public async Task<ActionResult> GetMessages(int studentId)
        {
            var messages = await studentService.GetStudentMessages(studentId);
            if (messages == null)
                return NotFound("No messages found");
            return Ok(messages);
        }
    }
}