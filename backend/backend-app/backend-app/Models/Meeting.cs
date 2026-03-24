using backend_app.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_app.Models
{
    [Table("meetings")]
    public class Meeting : Actionable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int OrganiserId { get; set; }

        [Required, MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public MeetingStatus Status { get; set; } = MeetingStatus.Scheduled;

        [Required]
        public DateTime ScheduledAt { get; set; }

        public int? DurationMins { get; set; }

        [MaxLength(500)]
        public string? LocationOrUrl { get; set; }

        public string? CancellationReason { get; set; }


        [ForeignKey(nameof(OrganiserId))]
        public User Organiser { get; set; } = null!;

        public ICollection<MeetingParticipant> Participants { get; set; } = new List<MeetingParticipant>();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}