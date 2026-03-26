namespace backend_app.DTOs
{
    public class StudentTutorDTO
    {
        public int TutorId { get; set; }
        public string TutorName { get; set; }
        public string TutorEmail { get; set; }
        public string TutorNotes { get; set; }
        public DateTime AllocatedAt { get; set; }
    }
}
