namespace backend_app.DTOs
{
    public class StudentTutorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public DateTime AllocatedAt { get; set; }
    }
}
