using backend_app.Attributes;
using backend_app.DTOs;
using backend_app.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ServiceFilter(typeof(AuthenticationAttribute))]
    public class DocumentController(DocumentUploadService documentUploadService) : ControllerBase
    {

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UploadFile([FromForm] UploadRequest uploadRequest)
        {
            var uploadResult = await documentUploadService.UploadFile(uploadRequest);

            return Ok(uploadResult);
        }

        [HttpGet("download/{id}")]
        public async Task<ActionResult> DownloadFile(int id)
        {
            var fileContent = await documentUploadService.DownloadFile(id);

            return fileContent != null ? File(Convert.FromBase64String(fileContent.Content), fileContent.MimePath, fileContent.Filename) : NotFound("Document not found");
        }

        [HttpGet("my-documents")]
        public async Task<ActionResult> GetAllDocumentRecords()
        {
            var documentRecords = await documentUploadService.GetAllDocumentRecords();

            return Ok(documentRecords);
        }

        [HttpGet("meeting/{id}")]
        public async Task<ActionResult> GetAllDocumentRecordsByMeetingId(int id)
        {
            var documentRecords = await documentUploadService.GetAllDocumentRecordsByMeetingId(id);

            return Ok(documentRecords);
        }
    }
}
