using backend_app.Enums;

namespace backend_app.DTOs
{
    public class CreateMeetingDTO
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public MeetingType MeetingType { get; set; }
        public DateTime ScheduledAt { get; set; }
        public int DurationMinutes { get; set; }
        public string? LocationOrUrl { get; set; }
        public List<int> ParticipantIds { get; set; } = [];
    }
}