namespace backend_app.DTOs
{
    public class DocumentResponse
    {
        public int Id { get; set; }
        public int UploaderId { get; set; }

        public int MeetingId { get; set; }

        public string FileName { get; set; }

        public string MimePath { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

    }
}
