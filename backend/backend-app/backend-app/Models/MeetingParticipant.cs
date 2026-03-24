using backend_app.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_app.Models
{

    [Table("meeting_participants")]
    public class MeetingParticipant : Actionable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int MeetingId { get; set; }

        [Required]
        public int UserId { get; set; }

        public AttendanceStatus AttendanceStatus { get; set; } = AttendanceStatus.Pending;

        public DateTime? RespondedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(MeetingId))]
        public Meeting Meeting { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;
    }
}