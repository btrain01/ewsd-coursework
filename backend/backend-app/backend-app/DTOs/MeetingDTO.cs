using backend_app.Enums;

namespace backend_app.DTOs
{
    public class MeetingDTO
    {
        public int Id { get; set; } 
        public DateTime ScheduledAt { get; set; } 
        public int DurationInMinutes { get; set; } 
        public MeetingType MeetingType { get; set; } 
        public string MeetingLink { get; set; } 
        public string Agenda { get; set; } 
        public MeetingStatus MeetingStatus { get; set; } 
    }
}
