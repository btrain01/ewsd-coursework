namespace backend_app.DTOs
{
    public class DocumentDTO
    {
        public int Id { get; set; } 
        public string FileName { get; set; } 
        public string Description { get; set; } 
        public string MimeType { get; set; } 
        public float FileSizeBytes { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
