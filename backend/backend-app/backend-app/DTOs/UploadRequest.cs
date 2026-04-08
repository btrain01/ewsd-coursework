namespace backend_app.DTOs
{
    public class UploadRequest
    {
        public int MeetingId { get; set; }

        public IFormFile File { get; set; }
    }
}
    